import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { ClinicApi } from '../../core/clinic-api';
import { fullName, readApiError } from '../../core/labels';
import { Doctor, Specialty } from '../../core/models';
import { specialtyLabel } from '../../core/specialty-visual';
import { Pager } from '../../shared/pager/pager';
import { SpecialtyMark } from '../../shared/specialty-mark/specialty-mark';

@Component({
  selector: 'app-doctors',
  imports: [ReactiveFormsModule, SpecialtyMark, Pager],
  templateUrl: './doctors.html',
})
export class Doctors {
  private readonly api = inject(ClinicApi);
  private readonly formBuilder = inject(FormBuilder);

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
  protected readonly specialtyOptions = signal<Specialty[]>([]);
  protected readonly specialtyName = specialtyLabel;
  protected readonly addingSpecialty = signal(false);
  protected readonly customSpecialty = signal('');
  protected readonly form = this.formBuilder.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    specialtyId: ['', Validators.required],
    licenseNumber: ['', Validators.required],
    phone: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
  });

  constructor() {
    void this.load();
  }

  protected onSearch(event: Event): void {
    this.search.set((event.target as HTMLInputElement).value);
  }

  protected openCreate(): void {
    this.editingId.set(null);
    this.form.reset();
    this.addingSpecialty.set(false);
    this.customSpecialty.set('');
    this.editorOpen.set(true);
  }

  protected selectedSpecialtyName(): string {
    const id = this.form.controls.specialtyId.value;
    return this.specialtyOptions().find((item) => item.id === id)?.name ?? '';
  }

  protected openEdit(doctor: Doctor): void {
    this.editingId.set(doctor.id);
    this.ensureSpecialty(doctor);
    this.addingSpecialty.set(false);
    this.customSpecialty.set('');
    this.form.setValue({
      firstName: doctor.firstName,
      lastName: doctor.lastName,
      specialtyId: doctor.specialtyId,
      licenseNumber: doctor.licenseNumber,
      phone: doctor.phone,
      email: doctor.email,
    });
    this.editorOpen.set(true);
  }

  protected close(): void {
    this.editorOpen.set(false);
    this.addingSpecialty.set(false);
  }

  protected toggleCustomSpecialty(): void {
    this.addingSpecialty.update((open) => !open);
    this.customSpecialty.set('');
  }

  protected onCustomSpecialty(event: Event): void {
    this.customSpecialty.set((event.target as HTMLInputElement).value);
  }

  protected async addSpecialty(): Promise<void> {
    const text = this.customSpecialty().trim().replace(/\s+/g, ' ');
    if (!text) return;

    const known = this.specialtyOptions().find((item) => item.name.replace(/\u200c/g, '') === text.replace(/\u200c/g, ''));
    if (known) {
      this.form.controls.specialtyId.setValue(known.id);
      this.addingSpecialty.set(false);
      this.customSpecialty.set('');
      return;
    }

    this.saving.set(true);
    this.error.set(null);
    try {
      const created = await firstValueFrom(this.api.createSpecialty(text));
      this.specialtyOptions.update((items) =>
        [...items.filter((item) => item.id !== created.id), created].sort((left, right) => left.name.localeCompare(right.name, 'fa')),
      );
      this.form.controls.specialtyId.setValue(created.id);
      this.addingSpecialty.set(false);
      this.customSpecialty.set('');
    } catch (error) {
      this.error.set(readApiError(error));
    } finally {
      this.saving.set(false);
    }
  }

  private ensureSpecialty(doctor: Doctor): void {
    if (!doctor.specialtyId || this.specialtyOptions().some((item) => item.id === doctor.specialtyId)) return;
    this.specialtyOptions.update((items) => [...items, { id: doctor.specialtyId, name: doctor.specialty }]);
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
      const [doctors, specialties] = await Promise.all([
        firstValueFrom(this.api.doctors(this.search(), this.page())),
        firstValueFrom(this.api.specialties()),
      ]);
      this.doctors.set(doctors.items);
      this.page.set(doctors.page);
      this.totalPages.set(doctors.totalPages);
      this.specialtyOptions.set(specialties);
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
    const body = this.form.getRawValue();
    try {
      const id = this.editingId();
      if (id) {
        await firstValueFrom(this.api.updateDoctor(id, body));
      } else {
        await firstValueFrom(this.api.createDoctor(body));
      }
      this.close();
      await this.load();
    } catch (error) {
      this.error.set(readApiError(error));
    } finally {
      this.saving.set(false);
    }
  }

  protected async remove(doctor: Doctor): Promise<void> {
    if (!window.confirm(`پرونده ${fullName(doctor.firstName, doctor.lastName)} حذف شود؟`)) {
      return;
    }

    this.error.set(null);
    try {
      await firstValueFrom(this.api.deleteDoctor(doctor.id));
      await this.load();
    } catch (error) {
      this.error.set(readApiError(error));
    }
  }
}
