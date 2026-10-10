import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { ClinicApi } from '../../core/clinic-api';
import { fullName, readApiError } from '../../core/labels';
import { Doctor } from '../../core/models';
import { SpecialtyMark } from '../../shared/specialty-mark/specialty-mark';

@Component({
  selector: 'app-doctors',
  imports: [ReactiveFormsModule, SpecialtyMark],
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
  protected readonly editorOpen = signal(false);
  protected readonly editingId = signal<string | null>(null);
  protected readonly name = fullName;
  protected readonly form = this.formBuilder.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    specialty: ['', Validators.required],
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
    this.editorOpen.set(true);
  }

  protected openEdit(doctor: Doctor): void {
    this.editingId.set(doctor.id);
    this.form.setValue({
      firstName: doctor.firstName,
      lastName: doctor.lastName,
      specialty: doctor.specialty,
      licenseNumber: doctor.licenseNumber,
      phone: doctor.phone,
      email: doctor.email,
    });
    this.editorOpen.set(true);
  }

  protected close(): void {
    this.editorOpen.set(false);
  }

  protected async load(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      this.doctors.set(await firstValueFrom(this.api.doctors(this.search())));
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
