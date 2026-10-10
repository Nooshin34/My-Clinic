import { HttpErrorResponse } from '@angular/common/http';

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
  const parts = new Intl.DateTimeFormat('fa-IR-u-ca-persian', {
    weekday: 'long',
    day: 'numeric',
    month: 'long',
    year: 'numeric',
  }).formatToParts(date);
  const value = (type: Intl.DateTimeFormatPartTypes) => parts.find((part) => part.type === type)?.value ?? '';
  return `${value('weekday')} ${value('day')} ${value('month')} ${value('year')}`;
}

export function formatWhen(value: string): string {
  return new Intl.DateTimeFormat('fa-IR', {
    dateStyle: 'medium',
    timeStyle: 'short',
  }).format(new Date(value));
}

export function toLocalInput(value: string): string {
  const date = new Date(value);
  const pad = (part: number) => String(part).padStart(2, '0');
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
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
