import { Component, effect, signal } from '@angular/core';
import { JobSearchBarComponent } from '../search/job-search-bar.component';
import { JobBoardComponent } from '../board/job-board.component';
import { JobSearchParams } from '../../models/job-search-params.model';
import { JobFiltersComponent } from '../filters/job-filters.component';

@Component({
  selector: 'app-job-page',
  imports: [JobSearchBarComponent, JobBoardComponent, JobFiltersComponent],
  templateUrl: './job-page.component.html',
  styleUrl: './job-page.component.scss',
})
export class JobPageComponent {
  filtersVisible = signal(true);

  constructor() {
    effect(() => {
      console.log(`Filters visibility changed: ${this.filtersVisible()}`);
    });
  }

  toggleFilters() {
    this.filtersVisible.update((visible) => !visible);
  }
}
