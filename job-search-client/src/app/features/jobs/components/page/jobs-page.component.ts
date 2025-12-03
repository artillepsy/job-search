import { Component, signal } from '@angular/core';
import { JobSearchPanelComponent } from '../search/job-search-panel.component';
import { JobItemsComponent } from '../job-items/job-items.component';
import { JobSearchParams } from '../../models/job-search.params.model';
import { FiltersDropdownComponent } from '../filters/filters-dropdown.component';

@Component({
  selector: 'app-jobs-page',
  imports: [JobSearchPanelComponent, JobItemsComponent, FiltersDropdownComponent],
  templateUrl: './jobs-page.component.html',
  styleUrl: './jobs-page.component.scss',
})
export class JobsPageComponent {
  isFiltersShown = false;

  searchParams = signal<JobSearchParams>({
    jobTitle: '',
    country: '',
  });

  updateSearchParams(params: JobSearchParams) {
    this.searchParams.set(params);
  }

  toggleFilters() {
    this.isFiltersShown = !this.isFiltersShown;
    console.log(`toggleFilters. isFiltersShown: ${this.isFiltersShown}`);
  }
}
