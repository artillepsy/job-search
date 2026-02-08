import { Component, inject} from '@angular/core';
import { Checkbox } from 'primeng/checkbox';
import { JobUrlService } from '../../services/url/job-url.service';
import { FormsModule } from '@angular/forms';
import { JobSearchParams } from '../../models/job-search-params.model';

@Component({
  selector: 'app-job-filters',
  imports: [Checkbox, FormsModule],
  templateUrl: './job-filters.component.html',
  styleUrl: './job-filters.component.scss',
})
export class JobFiltersComponent {
  private _urlService = inject(JobUrlService);

  withSalaryOnly = this._urlService.params().withSalaryOnly;
  isRemoteOnly = this._urlService.params().isRemoteOnly;

  isCareersInPoland = this._urlService.params().isCareersInPoland;
  isUsaJobs = this._urlService.params().isUsaJobs;
  isArbeitnow = this._urlService.params().isArbeitnow;

  onFilterChange(key: keyof JobSearchParams, value: boolean) {
    this._urlService.updateSearch({ [key]: value });
  }
}
