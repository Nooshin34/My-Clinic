import { Component, forwardRef, input, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import {
  gregorianToJalali,
  jalaliMonthLength,
  jalaliMonths,
  jalaliToGregorian,
  toIsoDate,
  toPersianDigits,
} from '../../core/jalali';

@Component({
  selector: 'app-shamsi-date',
  templateUrl: './shamsi-date.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ShamsiDate),
      multi: true,
    },
  ],
})
export class ShamsiDate implements ControlValueAccessor {
  readonly withTime = input(false);

  protected readonly months = jalaliMonths;
  protected readonly digits = toPersianDigits;
  protected readonly clock = (value: number) => toPersianDigits(String(value).padStart(2, '0'));
  protected readonly disabled = signal(false);
  protected readonly year = signal<number | null>(null);
  protected readonly month = signal<number | null>(null);
  protected readonly day = signal<number | null>(null);
  protected readonly hour = signal(8);
  protected readonly minute = signal(0);

  private onChange: (value: string) => void = () => undefined;
  private onTouched: () => void = () => undefined;

  writeValue(value: string | null): void {
    this.apply(value ?? '', this.withTime());
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  protected years(): number[] {
    const today = gregorianToJalali(new Date().getFullYear(), new Date().getMonth() + 1, new Date().getDate());
    const past = this.withTime() ? 2 : 120;
    const future = this.withTime() ? 3 : 0;
    const years: number[] = [];
    for (let year = today.year + future; year >= today.year - past; year -= 1) years.push(year);
    const selected = this.year();
    if (selected !== null && !years.includes(selected)) years.push(selected);
    return years.sort((left, right) => right - left);
  }

  protected days(): number[] {
    const year = this.year();
    const month = this.month();
    const length = year && month ? jalaliMonthLength(year, month) : 31;
    return Array.from({ length }, (_, index) => index + 1);
  }

  protected hours(): number[] {
    return Array.from({ length: 24 }, (_, hour) => hour);
  }

  protected minutes(): number[] {
    const values = Array.from({ length: 12 }, (_, index) => index * 5);
    const current = this.minute();
    if (!values.includes(current)) values.push(current);
    return values.sort((left, right) => left - right);
  }

  protected onDay(event: Event): void {
    this.day.set(readNumber(event));
    this.publish();
  }

  protected onMonth(event: Event): void {
    this.month.set(readNumber(event));
    this.clampDay();
    this.publish();
  }

  protected onYear(event: Event): void {
    this.year.set(readNumber(event));
    this.clampDay();
    this.publish();
  }

  protected onHour(event: Event): void {
    this.hour.set(readNumber(event) ?? 0);
    this.publish();
  }

  protected onMinute(event: Event): void {
    this.minute.set(readNumber(event) ?? 0);
    this.publish();
  }

  private apply(value: string, withTime: boolean): void {
    if (!value) {
      this.year.set(null);
      this.month.set(null);
      this.day.set(null);
      this.hour.set(8);
      this.minute.set(0);
      return;
    }

    if (!withTime) {
      const match = /^(\d{4})-(\d{2})-(\d{2})/.exec(value);
      if (!match) return;
      const jalali = gregorianToJalali(Number(match[1]), Number(match[2]), Number(match[3]));
      this.year.set(jalali.year);
      this.month.set(jalali.month);
      this.day.set(jalali.day);
      return;
    }

    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return;
    const jalali = gregorianToJalali(date.getFullYear(), date.getMonth() + 1, date.getDate());
    this.year.set(jalali.year);
    this.month.set(jalali.month);
    this.day.set(jalali.day);
    this.hour.set(date.getHours());
    this.minute.set(date.getMinutes());
  }

  private clampDay(): void {
    const year = this.year();
    const month = this.month();
    const day = this.day();
    if (!year || !month || !day) return;
    const length = jalaliMonthLength(year, month);
    if (day > length) this.day.set(length);
  }

  private publish(): void {
    const year = this.year();
    const month = this.month();
    const day = this.day();
    this.onTouched();
    if (!year || !month || !day) {
      this.onChange('');
      return;
    }

    const gregorian = jalaliToGregorian(year, month, day);
    if (!this.withTime()) {
      this.onChange(toIsoDate(gregorian));
      return;
    }

    const local = new Date(gregorian.year, gregorian.month - 1, gregorian.day, this.hour(), this.minute(), 0, 0);
    this.onChange(local.toISOString());
  }
}

function readNumber(event: Event): number | null {
  const value = (event.target as HTMLSelectElement).value;
  return value ? Number(value) : null;
}
