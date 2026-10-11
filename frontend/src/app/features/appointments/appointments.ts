import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { ClinicApi } from '../../core/clinic-api';
import { formatWhen, fullName, readApiError, statusLabel } from '../../core/labels';
import { Appointment, Doctor, Patient } from '../../core/models';
import { specialtyLabel } from '../../core/specialty-visual';
import { Pager } from '../../shared/pager/pager';
import { ShamsiDate } from '../../shared/shamsi-date/shamsi-date';
import { SpecialtyMark } from '../../shared/specialty-mark/specialty-mark';

@Component({
  selector: 'app-appointments',
  imports: [ReactiveFormsModule, SpecialtyMark, ShamsiDate, Pager],
  templateUrl: './appointments.html',
})
export class Appointments {
  private readonly api = inject(ClinicApi);
  private readonly formBuilder = inject(FormBuilder);

  protected readonly appointments = signal<Appointment[]>([]);
  protected readonly patients = signal<Patient[]>([]);
  protected readonly doctors = signal<Doctor[]>([]);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly search = signal('');
  protected readonly page = signal(1);
  protected readonly totalPages = signal(0);
  protected readonly editorOpen = signal(false);
  protected readonly editingId = signal<string | null>(null);
  protected readonly name = fullName;
  protected readonly when = formatWhen;
  protected readonly status = statusLabel;
  protected readonly specialtyName = specialtyLabel;
  protected readonly form = this.formBuilder.nonNullable.group({
    patientId: ['', Validators.required],
    doctorId: ['', Validators.required],
    scheduledAt: ['', Validators.required],
    reason: ['', Validators.required],
    status: ['Scheduled', Validators.required],
    notes: [''],
  });

  protected selectedSpecialty(): string | null {
    const id = this.form.controls.doctorId.value;
    return this.doctors().find((doctor) => doctor.id === id)?.specialty ?? null;
  }

  constructor() {
    void this.load();
  }

  protected onSearch(event: Event): void {
    this.search.set((event.target as HTMLInputElement).value);
  }

  protected openCreate(): void {
    this.editingId.set(null);
    this.form.reset({ status: 'Scheduled', patientId: '', doctorId: '', notes: '' });
    this.editorOpen.set(true);
  }

  protected openEdit(item: Appointment): void {
    this.editingId.set(item.id);
    this.form.setValue({
      patientId: item.patientId,
      doctorId: item.doctorId,
      scheduledAt: item.scheduledAtUtc,
      reason: item.reason,
      status: item.status,
      notes: item.notes ?? '',
    });
    this.editorOpen.set(true);
  }

  protected close(): void {
    this.editorOpen.set(false);
  }

  protected searchList(): void {
    this.page.set(1);
    void this.load();
  }

  protected goTo(page: number): void {
    this.page.set(page);
    void this.load();
  }

  protected async load(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      const [appointments, patients, doctors] = await Promise.all([
        firstValueFrom(this.api.appointments(this.search(), this.page())),
        this.api.patientOptions(),
        this.api.doctorOptions(),
      ]);
      this.appointments.set(appointments.items);
      this.page.set(appointments.page);
      this.totalPages.set(appointments.totalPages);
      this.patients.set(patients);
      this.doctors.set(doctors);
    } catch (error) {
      this.error.set(readApiError(error));
    } finally {
      this.loading.set(false);
    }
  }

  protected async save(): Promise<void> {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.error.set(null);
    const raw = this.form.getRawValue();
    const body = {
      patientId: raw.patientId,
      doctorId: raw.doctorId,
      scheduledAtUtc: raw.scheduledAt,
      reason: raw.reason,
      status: raw.status,
      notes: raw.notes.trim() ? raw.notes.trim() : null,
    };

    try {
      const id = this.editingId();
      if (id) {
        await firstValueFrom(this.api.updateAppointment(id, body));
      } else {
        await firstValueFrom(this.api.createAppointment(body));
      }
      this.close();
      await this.load();
    } catch (error) {
      this.error.set(readApiError(error));
    } finally {
      this.saving.set(false);
    }
  }

  protected async remove(item: Appointment): Promise<void> {
    if (!window.confirm(`نوبت ${item.patientName} حذف شود؟`)) {
      return;
    }

    this.error.set(null);
    try {
      await firstValueFrom(this.api.deleteAppointment(item.id));
      await this.load();
    } catch (error) {
      this.error.set(readApiError(error));
    }
  }
}
