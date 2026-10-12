import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { firstValueFrom, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  Appointment,
  AppointmentInput,
  AvailabilityInput,
  AvailabilitySlot,
  Dashboard,
  Doctor,
  DoctorInput,
  Page,
  Patient,
  PatientInput,
  Specialty,
} from './models';

export const listPageSize = 10;

@Injectable({ providedIn: 'root' })
export class ClinicApi {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiUrl;

  dashboard() {
    return this.http.get<Dashboard>(`${this.base}/dashboard`);
  }

  patients(search?: string, page = 1, pageSize = listPageSize) {
    return this.http.get<Page<Patient>>(`${this.base}/patients`, { params: pageParams(search, page, pageSize) });
  }

  patientOptions() {
    return this.everyPage((page) => this.patients(undefined, page, 100));
  }

  createPatient(body: PatientInput) {
    return this.http.post<Patient>(`${this.base}/patients`, body);
  }

  updatePatient(id: string, body: PatientInput) {
    return this.http.put<Patient>(`${this.base}/patients/${id}`, body);
  }

  deletePatient(id: string) {
    return this.http.delete<void>(`${this.base}/patients/${id}`);
  }

  doctors(search?: string, page = 1, pageSize = listPageSize) {
    return this.http.get<Page<Doctor>>(`${this.base}/doctors`, { params: pageParams(search, page, pageSize) });
  }

  doctorOptions() {
    return this.everyPage((page) => this.doctors(undefined, page, 100));
  }

  createDoctor(body: DoctorInput) {
    return this.http.post<Doctor>(`${this.base}/doctors`, body);
  }

  updateDoctor(id: string, body: DoctorInput) {
    return this.http.put<Doctor>(`${this.base}/doctors/${id}`, body);
  }

  deleteDoctor(id: string) {
    return this.http.delete<void>(`${this.base}/doctors/${id}`);
  }

  specialties() {
    return this.http.get<Specialty[]>(`${this.base}/specialties`);
  }

  createSpecialty(name: string) {
    return this.http.post<Specialty>(`${this.base}/specialties`, { name });
  }

  appointments(search?: string, page = 1, pageSize = listPageSize) {
    return this.http.get<Page<Appointment>>(`${this.base}/appointments`, { params: pageParams(search, page, pageSize) });
  }

  createAppointment(body: AppointmentInput) {
    return this.http.post<Appointment>(`${this.base}/appointments`, body);
  }

  updateAppointment(id: string, body: AppointmentInput) {
    return this.http.put<Appointment>(`${this.base}/appointments/${id}`, body);
  }

  deleteAppointment(id: string) {
    return this.http.delete<void>(`${this.base}/appointments/${id}`);
  }

  availability(doctorId: string, from: string, to: string) {
    const params = new HttpParams().set('from', from).set('to', to);
    return this.http.get<AvailabilitySlot[]>(`${this.base}/doctors/${doctorId}/availability`, { params });
  }

  createAvailability(doctorId: string, body: AvailabilityInput) {
    return this.http.post<AvailabilitySlot[]>(`${this.base}/doctors/${doctorId}/availability`, body);
  }

  deleteAvailability(id: string) {
    return this.http.delete<void>(`${this.base}/availability/${id}`);
  }

  private async everyPage<T>(load: (page: number) => Observable<Page<T>>): Promise<T[]> {
    const first = await firstValueFrom(load(1));
    if (first.totalPages <= 1) return first.items;

    const rest = await Promise.all(
      Array.from({ length: first.totalPages - 1 }, (_, index) => firstValueFrom(load(index + 2))),
    );
    return first.items.concat(...rest.map((page) => page.items));
  }
}

function pageParams(search: string | undefined, page: number, pageSize: number): HttpParams {
  let params = new HttpParams().set('page', page).set('pageSize', pageSize);
  const term = search?.trim();
  if (term) params = params.set('search', term);
  return params;
}

