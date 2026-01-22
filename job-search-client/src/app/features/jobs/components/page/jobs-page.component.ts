import { Component, signal } from '@angular/core';
import { JobSearchPanelComponent } from '../search/job-search-panel.component';
import { JobItemsBoardComponent } from '../job-items-board/job-items-board.component';
import { JobSearchParams } from '../../models/job-search.params.model';
import { FiltersDropdownComponent } from '../filters/filters-dropdown.component';

@Component({
  selector: 'app-jobs-page',
  imports: [JobSearchPanelComponent, JobItemsBoardComponent, FiltersDropdownComponent],
  templateUrl: './jobs-page.component.html',
  styleUrl: './jobs-page.component.scss',
})
export class JobsPageComponent {
  isFiltersShown = false;

  searchParams = signal<JobSearchParams>({});

  updateSearchParams(params: JobSearchParams) {
    this.searchParams.set(params);
  }

  toggleFilters() {
    this.isFiltersShown = !this.isFiltersShown;
    console.log(`toggleFilters. isFiltersShown: ${this.isFiltersShown}`);
  }
}
