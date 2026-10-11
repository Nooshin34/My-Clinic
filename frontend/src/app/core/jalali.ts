export interface CalendarDate {
  year: number;
  month: number;
  day: number;
}

export const jalaliMonths = [
  'فروردین',
  'اردیبهشت',
  'خرداد',
  'تیر',
  'مرداد',
  'شهریور',
  'مهر',
  'آبان',
  'آذر',
  'دی',
  'بهمن',
  'اسفند',
] as const;

const weekdays = ['یکشنبه', 'دوشنبه', 'سه‌شنبه', 'چهارشنبه', 'پنجشنبه', 'جمعه', 'شنبه'];

const persianDigits = '۰۱۲۳۴۵۶۷۸۹';

export function toPersianDigits(value: number | string): string {
  return String(value).replace(/\d/g, (digit) => persianDigits[Number(digit)] ?? digit);
}

export function gregorianToJalali(year: number, month: number, day: number): CalendarDate {
  return julianDayToJalali(gregorianToJulianDay(year, month, day));
}

export function jalaliToGregorian(year: number, month: number, day: number): CalendarDate {
  return julianDayToGregorian(jalaliToJulianDay(year, month, day));
}

export function isJalaliLeapYear(year: number): boolean {
  return jalaliCalendar(year).leap === 0;
}

export function jalaliMonthLength(year: number, month: number): number {
  if (month <= 6) return 31;
  if (month <= 11) return 30;
  return isJalaliLeapYear(year) ? 30 : 29;
}

export function formatShamsiDate(value: string): string {
  const match = /^(\d{4})-(\d{2})-(\d{2})/.exec(value);
  if (!match) return value;

  try {
    return formatJalaliParts(gregorianToJalali(Number(match[1]), Number(match[2]), Number(match[3])));
  } catch {
    return value;
  }
}

export function formatShamsiDateTime(value: string): string {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;

  try {
    const jalali = gregorianToJalali(date.getFullYear(), date.getMonth() + 1, date.getDate());
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${formatJalaliParts(jalali)}، ${toPersianDigits(hours)}:${toPersianDigits(minutes)}`;
  } catch {
    return value;
  }
}

export function formatShamsiToday(date = new Date()): string {
  const jalali = gregorianToJalali(date.getFullYear(), date.getMonth() + 1, date.getDate());
  return `${weekdays[date.getDay()]} ${toPersianDigits(jalali.day)} ${jalaliMonths[jalali.month - 1]} ${toPersianDigits(jalali.year)}`;
}

export function toIsoDate(date: CalendarDate): string {
  return `${date.year}-${pad(date.month)}-${pad(date.day)}`;
}

function formatJalaliParts(date: CalendarDate): string {
  return `${toPersianDigits(date.day)} ${jalaliMonths[date.month - 1]} ${toPersianDigits(date.year)}`;
}

function pad(value: number): string {
  return String(value).padStart(2, '0');
}

function div(a: number, b: number): number {
  return Math.trunc(a / b);
}

function mod(a: number, b: number): number {
  return a - div(a, b) * b;
}

function jalaliCalendar(year: number): { leap: number; gregorianYear: number; march: number } {
  const breaks = [-61, 9, 38, 199, 426, 686, 756, 818, 1111, 1181, 1210, 1635, 2060, 2097, 2192, 2262, 2324, 2394, 2456, 3178];
  const gregorianYear = year + 621;
  let leapJ = -14;
  let jp = breaks[0];
  let jump = 0;

  if (year < jp || year >= breaks[breaks.length - 1]) {
    throw new Error(`Jalali year out of range: ${year}`);
  }

  for (let index = 1; index < breaks.length; index += 1) {
    const jm = breaks[index];
    jump = jm - jp;
    if (year < jm) break;
    leapJ += div(jump, 33) * 8 + div(mod(jump, 33), 4);
    jp = jm;
  }

  let n = year - jp;
  leapJ += div(n, 33) * 8 + div(mod(n, 33) + 3, 4);
  if (mod(jump, 33) === 4 && jump - n === 4) leapJ += 1;

  const leapG = div(gregorianYear, 4) - div((div(gregorianYear, 100) + 1) * 3, 4) - 150;
  const march = 20 + leapJ - leapG;

  return { leap: leapFromCycle(jump, n), gregorianYear, march };
}

function leapFromCycle(jump: number, offset: number): number {
  let adjusted = offset;
  if (jump - offset < 6) adjusted = offset - jump + div(jump + 4, 33) * 33;
  let leap = mod(mod(adjusted + 1, 33) - 1, 4);
  if (leap === -1) leap = 4;
  return leap;
}

function gregorianToJulianDay(year: number, month: number, day: number): number {
  let julian =
    div((year + div(month - 8, 6) + 100100) * 1461, 4) +
    div(153 * mod(month + 9, 12) + 2, 5) +
    day -
    34840408;
  julian = julian - div(div(year + 100100 + div(month - 8, 6), 100) * 3, 4) + 752;
  return julian;
}

function julianDayToGregorian(julianDay: number): CalendarDate {
  let j = 4 * julianDay + 139361631;
  j = j + div(div(4 * julianDay + 183187720, 146097) * 3, 4) * 4 - 3908;
  const i = div(mod(j, 1461), 4) * 5 + 308;
  const day = div(mod(i, 153), 5) + 1;
  const month = mod(div(i, 153), 12) + 1;
  const year = div(j, 1461) - 100100 + div(8 - month, 6);
  return { year, month, day };
}

function jalaliToJulianDay(year: number, month: number, day: number): number {
  const calendar = jalaliCalendar(year);
  return gregorianToJulianDay(calendar.gregorianYear, 3, calendar.march) + (month - 1) * 31 - div(month, 7) * (month - 7) + day - 1;
}

function julianDayToJalali(julianDay: number): CalendarDate {
  const gregorianYear = julianDayToGregorian(julianDay).year;
  let year = gregorianYear - 621;
  const calendar = jalaliCalendar(year);
  const farvardin = gregorianToJulianDay(gregorianYear, 3, calendar.march);
  let dayOfYear = julianDay - farvardin;

  if (dayOfYear >= 0) {
    if (dayOfYear <= 185) {
      return { year, month: 1 + div(dayOfYear, 31), day: mod(dayOfYear, 31) + 1 };
    }
    dayOfYear -= 186;
  } else {
    year -= 1;
    dayOfYear += 179;
    if (calendar.leap === 1) dayOfYear += 1;
  }

  return { year, month: 7 + div(dayOfYear, 30), day: mod(dayOfYear, 30) + 1 };
}
