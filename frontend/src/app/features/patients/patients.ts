import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { ClinicApi } from '../../core/clinic-api';
import { formatBirthDate, fullName, readApiError } from '../../core/labels';
import { Patient } from '../../core/models';
import { GenderMark } from '../../shared/gender-mark/gender-mark';
import { ListLoading } from '../../shared/list-loading/list-loading';
import { Pager } from '../../shared/pager/pager';
import { ShamsiDate } from '../../shared/shamsi-date/shamsi-date';

@Component({
  selector: 'app-patients',
  imports: [ReactiveFormsModule, GenderMark, ShamsiDate, Pager, ListLoading],
  templateUrl: './patients.html',
})
export class Patients {
  private readonly api = inject(ClinicApi);
  private readonly formBuilder = inject(FormBuilder);

  protected readonly patients = signal<Patient[]>([]);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly search = signal('');
  protected readonly page = signal(1);
  protected readonly totalPages = signal(0);
  protected readonly editorOpen = signal(false);
  protected readonly editingId = signal<string | null>(null);
  protected readonly name = fullName;
  protected readonly birthDate = formatBirthDate;
  protected readonly form = this.formBuilder.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    nationalId: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
    phone: ['', Validators.required],
    dateOfBirth: ['', Validators.required],
    gender: ['Female', Validators.required],
    address: [''],
  });

  constructor() {
    void this.load();
  }

  protected onSearch(event: Event): void {
    this.search.set((event.target as HTMLInputElement).value);
  }

  protected openCreate(): void {
    this.editingId.set(null);
    this.form.reset({ gender: 'Female', address: '' });
    this.editorOpen.set(true);
  }

  protected openEdit(patient: Patient): void {
    this.editingId.set(patient.id);
    this.form.setValue({
      firstName: patient.firstName,
      lastName: patient.lastName,
      nationalId: patient.nationalId,
      phone: patient.phone,
      dateOfBirth: patient.dateOfBirth,
      gender: patient.gender,
      address: patient.address ?? '',
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
      const result = await firstValueFrom(this.api.patients(this.search(), this.page()));
      this.patients.set(result.items);
      this.page.set(result.page);
      this.totalPages.set(result.totalPages);
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
    const body = { ...raw, address: raw.address.trim() ? raw.address.trim() : null };
    try {
      const id = this.editingId();
      if (id) {
        await firstValueFrom(this.api.updatePatient(id, body));
      } else {
        await firstValueFrom(this.api.createPatient(body));
      }
      this.close();
      await this.load();
    } catch (error) {
      this.error.set(readApiError(error));
    } finally {
      this.saving.set(false);
    }
  }

  protected async remove(patient: Patient): Promise<void> {
    if (!window.confirm(`پرونده ${fullName(patient.firstName, patient.lastName)} حذف شود؟`)) {
      return;
    }

    this.error.set(null);
    try {
      await firstValueFrom(this.api.deletePatient(patient.id));
      await this.load();
    } catch (error) {
      this.error.set(readApiError(error));
    }
  }
}
