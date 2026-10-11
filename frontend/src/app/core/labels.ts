import { HttpErrorResponse } from '@angular/common/http';
import { formatShamsiDate, formatShamsiDateTime, formatShamsiToday } from './jalali';

export function genderLabel(value: string): string {
  switch (value) {
    case 'Female':
      return 'زن';
    case 'Male':
      return 'مرد';
    default:
      return 'نامشخص';
  }
}

export function statusLabel(value: string): string {
  switch (value) {
    case 'Completed':
      return 'انجام‌شده';
    case 'Cancelled':
      return 'لغو‌شده';
    default:
      return 'برنامه‌ریزی‌شده';
  }
}

export function fullName(firstName: string, lastName: string): string {
  return `${firstName} ${lastName}`;
}

export function todayShamsi(date = new Date()): string {
  return formatShamsiToday(date);
}

export function formatWhen(value: string): string {
  return formatShamsiDateTime(value);
}

export function formatBirthDate(value: string): string {
  return formatShamsiDate(value);
}

export function readApiError(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    const body = error.error as { title?: string; errors?: string[] } | null;
    if (body?.errors?.length) {
      return body.errors.join(' ');
    }
    if (body?.title) {
      return body.title;
    }
    if (error.status === 0) {
      return 'ارتباط با سرور برقرار نشد. API و PostgreSQL را بررسی کنید.';
    }
  }

  return 'خطای غیرمنتظره رخ داد.';
}
