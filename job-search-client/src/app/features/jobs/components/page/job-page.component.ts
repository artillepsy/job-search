import { Component,  signal } from '@angular/core';
import { JobSearchBarComponent } from '../search/job-search-bar.component';
import { JobBoardComponent } from '../board/job-board.component';

@Component({
  selector: 'app-job-page',
  imports: [JobSearchBarComponent, JobBoardComponent],
  templateUrl: './job-page.component.html',
  styleUrl: './job-page.component.scss',
})
export class JobPageComponent {
  filtersVisible = signal(true);

  toggleFilters() {
    this.filtersVisible.update((visible) => !visible);
  }
}
