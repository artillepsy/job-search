import { Component, effect, inject, input, output, signal } from '@angular/core';
import { JobSearchParams } from '../../models/job-search-params.model';
import { FormsModule } from '@angular/forms';
import { JobUrlService } from '../../services/url/job-url.service';

@Component({
  selector: 'app-search-bar',
  imports: [FormsModule],
  templateUrl: './job-search-bar.component.html',
  styleUrl: './job-search-bar.component.scss',
})
export class JobSearchBarComponent {
  private _urlService = inject(JobUrlService);

  filtersVisible = input<boolean>(false);
  toggleFilters = output();

  inputKeywords = signal<string>('');
  inputLocation = signal<string>('');

  constructor() {
    effect(() => {
      const params = this._urlService.params();
      this.inputKeywords.set(params?.keywords || '');
      this.inputLocation.set(params?.location || '');
    });
  }

  onClickSearch() {
    this._urlService.updateSearch({
      keywords: this.inputKeywords() || undefined,
      location: this.inputLocation() || undefined,
      pageNumber: 1, // reset page number to 1 on search
    });
  }

  onClickFilters() {
    this.toggleFilters.emit();
  }
}
