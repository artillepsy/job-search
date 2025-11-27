import { Component, signal } from '@angular/core';
import { JobSearchPanelComponent } from '../search/job-search-panel.component';
import { JobResultsComponent } from '../job-results/job-results.component';
import { JobSearchParams } from '../../models/job-search.params.model';

@Component({
  selector: 'app-jobs-page',
  imports: [JobSearchPanelComponent, JobResultsComponent],
  templateUrl: './jobs-page.component.html',
  styleUrl: './jobs-page.component.scss',
})
export class JobsPageComponent {
  searchParams = signal<JobSearchParams>({
    jobTitle: '',
    country: '',
  });

  updateSearchParams(params: JobSearchParams) {
    this.searchParams.set(params);
  }
}
