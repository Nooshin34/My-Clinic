import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./layout/shell').then((module) => module.Shell),
    children: [
      {
        path: '',
        loadComponent: () => import('./features/dashboard/dashboard').then((module) => module.Dashboard),
      },
      {
        path: 'patients',
        loadComponent: () => import('./features/patients/patients').then((module) => module.Patients),
      },
      {
        path: 'doctors',
        loadComponent: () => import('./features/doctors/doctors').then((module) => module.Doctors),
      },
      {
        path: 'appointments',
        loadComponent: () => import('./features/appointments/appointments').then((module) => module.Appointments),
      },
    ],
  },
];
