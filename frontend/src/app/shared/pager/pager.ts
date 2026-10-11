import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-pager',
  templateUrl: './pager.html',
})
export class Pager {
  readonly page = input.required<number>();
  readonly totalPages = input.required<number>();
  readonly pageChange = output<number>();

  protected go(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.pageChange.emit(page);
  }
}
