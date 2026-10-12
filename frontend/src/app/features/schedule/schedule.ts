import { Component, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ClinicApi } from '../../core/clinic-api';
import {
  addLocalDays,
  formatClock,
  formatShamsiDay,
  gregorianToJalali,
  isoDateLocal,
  toPersianDigits,
  weekdayName,
} from '../../core/jalali';
import { fullName, readApiError, visitKindLabel } from '../../core/labels';
import { AvailabilitySlot, Doctor } from '../../core/models';
import { specialtyLabel } from '../../core/specialty-visual';
import { ClinicArt } from '../../shared/clinic-art/clinic-art';

@Component({
  selector: 'app-schedule',
  imports: [ClinicArt],
  templateUrl: './schedule.html',
})
export class Schedule {
  private readonly api = inject(ClinicApi);

  protected readonly doctors = signal<Doctor[]>([]);
  protected readonly slots = signal<AvailabilitySlot[]>([]);
  protected readonly doctorId = signal('');
  protected readonly origin = signal(startOfToday());
  protected readonly selectedDate = signal(isoDateLocal(startOfToday()));
  protected readonly visitKind = signal<'InPerson' | 'Online'>('InPerson');
  protected readonly single = signal(false);
  protected readonly startHour = signal(9);
  protected readonly startMinute = signal(0);
  protected readonly endHour = signal(12);
  protected readonly endMinute = signal(0);
  protected readonly duration = signal(30);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly name = fullName;
  protected readonly specialtyName = specialtyLabel;
  protected readonly clock = formatClock;
  protected readonly visit = visitKindLabel;
  protected readonly digits = toPersianDigits;
  protected readonly pad = (value: number) => toPersianDigits(String(value).padStart(2, '0'));
  protected readonly hours = Array.from({ length: 15 }, (_, index) => index + 7);
  protected readonly minutes = [0, 15, 30, 45];
  protected readonly durations = [15, 20, 30, 45, 60];

  constructor() {
    void this.loadDoctors();
  }

  protected days(): Array<{ iso: string; weekday: string; label: string; count: number }> {
    return Array.from({ length: 14 }, (_, index) => {
      const date = addLocalDays(this.origin(), index);
      const iso = isoDateLocal(date);
      const jalali = gregorianToJalali(date.getFullYear(), date.getMonth() + 1, date.getDate());
      return {
        iso,
        weekday: weekdayName(date),
        label: toPersianDigits(jalali.day),
        count: this.slots().filter((slot) => slot.date === iso).length,
      };
    });
  }

  protected daySlots(): AvailabilitySlot[] {
    return this.slots().filter((slot) => slot.date === this.selectedDate());
  }

  protected selectedLabel(): string {
    const iso = this.selectedDate();
    return iso ? formatShamsiDay(iso) : '';
  }

  protected canGoBack(): boolean {
    return this.origin().getTime() > startOfToday().getTime();
  }

  protected chooseDoctor(event: Event): void {
    this.doctorId.set((event.target as HTMLSelectElement).value);
    void this.loadSlots();
  }

  protected pickDay(iso: string): void {
    this.selectedDate.set(iso);
  }

  protected setVisit(kind: 'InPerson' | 'Online'): void {
    this.visitKind.set(kind);
  }

  protected setSingle(event: Event): void {
    this.single.set((event.target as HTMLInputElement).checked);
  }

  protected setNumber(target: 'startHour' | 'startMinute' | 'endHour' | 'endMinute' | 'duration', event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this[target].set(value);
  }

  protected shift(days: number): void {
    const today = startOfToday();
    const max = addLocalDays(today, 76);
    let next = addLocalDays(this.origin(), days);
    if (next.getTime() < today.getTime()) next = today;
    if (next.getTime() > max.getTime()) next = max;
    this.origin.set(next);
    const selected = this.selectedDate();
    const windowEnd = isoDateLocal(addLocalDays(next, 13));
    if (!selected || selected < isoDateLocal(next) || selected > windowEnd) {
      this.selectedDate.set(isoDateLocal(next));
    }
    void this.loadSlots();
  }

  protected async save(): Promise<void> {
    const doctorId = this.doctorId();
    const date = this.selectedDate();
    if (!doctorId || !date) return;

    this.saving.set(true);
    this.error.set(null);
    try {
      await firstValueFrom(this.api.createAvailability(doctorId, {
        date,
        startTime: clockValue(this.startHour(), this.startMinute()),
        endTime: this.single() ? null : clockValue(this.endHour(), this.endMinute()),
        durationMinutes: this.duration(),
        visitKind: this.visitKind(),
      }));
      await this.loadSlots();
    } catch (error) {
      this.error.set(readApiError(error));
    } finally {
      this.saving.set(false);
    }
  }

  protected async remove(slot: AvailabilitySlot): Promise<void> {
    if (!window.confirm(`ساعت ${formatClock(slot.startTime)} حذف شود؟`)) return;

    this.error.set(null);
    try {
      await firstValueFrom(this.api.deleteAvailability(slot.id));
      await this.loadSlots();
    } catch (error) {
      this.error.set(readApiError(error));
    }
  }

  private async loadDoctors(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      this.doctors.set(await this.api.doctorOptions());
    } catch (error) {
      this.error.set(readApiError(error));
    } finally {
      this.loading.set(false);
    }
  }

  private async loadSlots(): Promise<void> {
    const doctorId = this.doctorId();
    if (!doctorId) {
      this.slots.set([]);
      return;
    }

    this.loading.set(true);
    this.error.set(null);
    try {
      const from = isoDateLocal(this.origin());
      const to = isoDateLocal(addLocalDays(this.origin(), 13));
      this.slots.set(await firstValueFrom(this.api.availability(doctorId, from, to)));
    } catch (error) {
      this.slots.set([]);
      this.error.set(readApiError(error));
    } finally {
      this.loading.set(false);
    }
  }
}

function startOfToday(): Date {
  const now = new Date();
  return new Date(now.getFullYear(), now.getMonth(), now.getDate());
}

function clockValue(hour: number, minute: number): string {
  return `${String(hour).padStart(2, '0')}:${String(minute).padStart(2, '0')}`;
}
