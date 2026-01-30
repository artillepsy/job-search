import { Component, input, output } from '@angular/core';
import { JobSearchParams } from '../../models/job-search.params.model';
import { FormsModule } from '@angular/forms';
import { JobSearchResults } from '../../models/job-search-results.model';

@Component({
  selector: 'app-search-panel',
  imports: [FormsModule],
  templateUrl: './job-search-panel.component.html',
  styleUrl: './job-search-panel.component.scss',
})
export class JobSearchPanelComponent {
  resultsCount = input<JobSearchResults | undefined>(undefined);

  searchChange = output<JobSearchParams>();
  filtersChange = output();

  inputKeywords: string | null = null;
  inputLocation: string | null = null;

  onClickSearch() {
    const params: JobSearchParams = {
      keywords: this.inputKeywords ?? undefined,
      location: this.inputLocation ?? undefined,
    };
    this.searchChange.emit(params);
  }

  onClickFilters() {
    this.filtersChange.emit();
  }
}
