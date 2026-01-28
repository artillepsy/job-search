import { Component, input, output, signal } from '@angular/core';
import { AutoComplete } from 'primeng/autocomplete';
import { Button } from 'primeng/button';
import { FloatLabel } from 'primeng/floatlabel';
import { JobSearchParams } from '../../models/job-search.params.model';
import { FormsModule } from '@angular/forms';
import { InputText } from 'primeng/inputtext';
import { JobSearchResults } from '../../models/job-search-results.model';
import { title } from '@primeuix/themes/aura/card';

@Component({
  selector: 'app-search-panel',
  imports: [FormsModule],
  templateUrl: './job-search-panel.component.html',
  styleUrl: './job-search-panel.component.scss',
})
export class JobSearchPanelComponent {
  searchChange = output<JobSearchParams>();
  filtersChange = output();
  resultsCount = input<JobSearchResults | undefined>(undefined);

  constructor() {}

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

  protected readonly title = title;
}
