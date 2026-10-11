import { Component, input } from '@angular/core';

export type ClinicArtKind = 'pill' | 'tablet' | 'syringe' | 'bottle' | 'bandage';

@Component({
  selector: 'app-clinic-art',
  templateUrl: './clinic-art.html',
  host: { 'aria-hidden': 'true' },
})
export class ClinicArt {
  readonly kind = input.required<ClinicArtKind>();
}
