import { Component, effect, inject, signal } from '@angular/core';
import { Checkbox } from 'primeng/checkbox';
import { JobUrlService } from '../../services/url/job-url.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-job-filters',
  imports: [Checkbox, FormsModule],
  templateUrl: './job-filters.component.html',
  styleUrl: './job-filters.component.scss',
})
export class JobFiltersComponent {
  private _urlService = inject(JobUrlService);

  isSalaryVisible = signal<boolean>(this._urlService.params().isSalaryVisible ?? false);
  isRemote = signal<boolean>(this._urlService.params().isRemote ?? false);

  isCareersInPoland = signal<boolean>(this._urlService.params().isCareersInPoland ?? true);
  isUsaJobs = signal<boolean>(this._urlService.params().isUsaJobs ?? true);
  isArbeitnow = signal<boolean>(this._urlService.params().isArbeitnow ?? true);

  constructor() {
    effect(() => {

      console.log("Effect values:", {
        poland: this.isCareersInPoland(),
        usa: this.isUsaJobs(),
        arbeit: this.isArbeitnow()
      });

      console.log("filters changed.");
      this._urlService.updateSearch({
        isSalaryVisible: this.isSalaryVisible(),
        isRemote: this.isRemote(),
        isCareersInPoland: this.isCareersInPoland(),
        isUsaJobs: this.isUsaJobs(),
        isArbeitnow: this.isArbeitnow(),
      });
    });
  }
}
