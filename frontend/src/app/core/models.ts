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

export interface Specialty {
  id: string;
  name: string;
}

export interface Doctor {
  id: string;
  firstName: string;
  lastName: string;
  specialtyId: string;
  specialty: string;
  phone: string;
  email: string;
  licenseNumber: string;
}

export interface DoctorInput {
  firstName: string;
  lastName: string;
  specialtyId: string;
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
  visitKind: string;
  notes: string | null;
}

export interface AppointmentInput {
  patientId: string;
  doctorId: string;
  scheduledAtUtc: string;
  reason: string;
  status: string;
  notes: string | null;
  visitKind: string | null;
}

export interface AvailabilitySlot {
  id: string;
  doctorId: string;
  date: string;
  startTime: string;
  durationMinutes: number;
  visitKind: string;
  startsAtUtc: string;
  booked: boolean;
}

export interface AvailabilityInput {
  date: string;
  startTime: string;
  endTime: string | null;
  durationMinutes: number;
  visitKind: string;
}

export interface Page<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface Dashboard {
  patientCount: number;
  doctorCount: number;
  todayAppointmentCount: number;
  scheduledAppointmentCount: number;
  todayAppointments: Appointment[];
}
