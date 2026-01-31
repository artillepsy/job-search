import { Component, input, output } from '@angular/core';
import { JobSearchParams } from '../../models/job-search-params.model';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-search-bar',
  imports: [FormsModule],
  templateUrl: './job-search-bar.component.html',
  styleUrl: './job-search-bar.component.scss',
})
export class JobSearchBarComponent {
  filtersVisible = input<boolean>(false);
  search = output<JobSearchParams>();
  toggleFilters = output();

  inputKeywords: string | null = null;
  inputLocation: string | null = null;

  onClickSearch() {
    const params: JobSearchParams = {
      keywords: this.inputKeywords ?? undefined,
      location: this.inputLocation ?? undefined,
    };
    this.search.emit(params);
  }

  onClickFilters() {
    this.toggleFilters.emit();
  }
}
