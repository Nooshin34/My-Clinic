import { Component, input } from '@angular/core';

@Component({
  selector: 'app-list-loading',
  templateUrl: './list-loading.html',
})
export class ListLoading {
  readonly label = input('در حال خواندن فهرست...');
  protected readonly rows = [0, 1, 2, 3];
}
