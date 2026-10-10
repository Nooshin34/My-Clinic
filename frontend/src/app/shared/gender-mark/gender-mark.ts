import { Component, computed, input } from '@angular/core';

@Component({
  selector: 'app-gender-mark',
  templateUrl: './gender-mark.html',
})
export class GenderMark {
  readonly gender = input.required<string>();
  protected readonly kind = computed(() => {
    switch (this.gender()) {
      case 'Female':
        return 'female';
      case 'Male':
        return 'male';
      default:
        return 'unknown';
    }
  });
}
