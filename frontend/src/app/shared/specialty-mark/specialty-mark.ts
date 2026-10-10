import { Component, computed, input } from '@angular/core';
import { specialtyVisual } from '../../core/specialty-visual';

@Component({
  selector: 'app-specialty-mark',
  templateUrl: './specialty-mark.html',
})
export class SpecialtyMark {
  readonly specialty = input.required<string>();
  protected readonly visual = computed(() => specialtyVisual(this.specialty()));
}
