export interface Patient {
  id: string;
  firstName: string;
  lastName: string;
  nationalId: string;
  phone: string;
  dateOfBirth: string;
  gender: string;
  address: string | null;
}

export interface PatientInput {
  firstName: string;
  lastName: string;
  nationalId: string;
  phone: string;
  dateOfBirth: string;
  gender: string;
  address: string | null;
}

export interface Doctor {
  id: string;
  firstName: string;
  lastName: string;
  specialty: string;
  phone: string;
  email: string;
  licenseNumber: string;
}

export interface DoctorInput {
  firstName: string;
  lastName: string;
  specialty: string;
  phone: string;
  email: string;
  licenseNumber: string;
}

export interface Appointment {
  id: string;
  patientId: string;
  patientName: string;
  doctorId: string;
  doctorName: string;
  doctorSpecialty: string;
  scheduledAtUtc: string;
  reason: string;
  status: string;
  notes: string | null;
}

export interface AppointmentInput {
  patientId: string;
  doctorId: string;
  scheduledAtUtc: string;
  reason: string;
  status: string;
  notes: string | null;
}

export interface Dashboard {
  patientCount: number;
  doctorCount: number;
  todayAppointmentCount: number;
  scheduledAppointmentCount: number;
  todayAppointments: Appointment[];
}
