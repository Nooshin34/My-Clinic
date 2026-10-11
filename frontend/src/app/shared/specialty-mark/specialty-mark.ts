import { Component, computed, input } from '@angular/core';
import { specialtyLabel, specialtyVisual } from '../../core/specialty-visual';

@Component({
  selector: 'app-specialty-mark',
  templateUrl: './specialty-mark.html',
})
export class SpecialtyMark {
  readonly specialty = input.required<string>();
  protected readonly visual = computed(() => specialtyVisual(this.specialty()));
  protected readonly title = computed(() => specialtyLabel(this.specialty()));
}
