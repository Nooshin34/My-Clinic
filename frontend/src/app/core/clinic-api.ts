import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  Appointment,
  AppointmentInput,
  Dashboard,
  Doctor,
  DoctorInput,
  Patient,
  PatientInput,
} from './models';

@Injectable({ providedIn: 'root' })
export class ClinicApi {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiUrl;

  dashboard() {
    return this.http.get<Dashboard>(`${this.base}/dashboard`);
  }

  patients(search?: string) {
    return this.http.get<Patient[]>(`${this.base}/patients`, { params: searchParams(search) });
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

  doctors(search?: string) {
    return this.http.get<Doctor[]>(`${this.base}/doctors`, { params: searchParams(search) });
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

  appointments(search?: string) {
    return this.http.get<Appointment[]>(`${this.base}/appointments`, { params: searchParams(search) });
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
}

function searchParams(search?: string): HttpParams {
  const term = search?.trim();
  return term ? new HttpParams().set('search', term) : new HttpParams();
}
