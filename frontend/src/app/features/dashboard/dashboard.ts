import { Component, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ClinicApi } from '../../core/clinic-api';
import { formatWhen, readApiError, statusLabel, todayShamsi } from '../../core/labels';
import { Dashboard as DashboardData } from '../../core/models';
import { specialtyLabel } from '../../core/specialty-visual';
import { SpecialtyMark } from '../../shared/specialty-mark/specialty-mark';

@Component({
  selector: 'app-dashboard',
  imports: [SpecialtyMark],
  templateUrl: './dashboard.html',
})
export class Dashboard {
  private readonly api = inject(ClinicApi);
  protected readonly data = signal<DashboardData | null>(null);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly when = formatWhen;
  protected readonly status = statusLabel;
  protected readonly today = todayShamsi();
  protected readonly specialtyName = specialtyLabel;

  constructor() {
    void this.load();
  }

  private async load(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      this.data.set(await firstValueFrom(this.api.dashboard()));
    } catch (error) {
      this.error.set(readApiError(error));
    } finally {
      this.loading.set(false);
    }
  }
}
