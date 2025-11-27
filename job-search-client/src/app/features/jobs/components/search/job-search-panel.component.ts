import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AutoComplete, AutoCompleteCompleteEvent } from 'primeng/autocomplete';
import { Button } from 'primeng/button';
import { FloatLabel } from 'primeng/floatlabel';
import { JobSearchParams } from '../../models/job-search.params.model';
import { COUNTRIES, Country } from '../../../../data/countries';
import { JOB_SUGGESTIONS } from '../../../../data/job-suggestions';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-search-panel',
  imports: [AutoComplete, Button, FloatLabel, FormsModule],
  templateUrl: './job-search-panel.component.html',
  styleUrl: './job-search-panel.component.scss',
})
export class JobSearchPanelComponent implements OnInit {
  @Output() search = new EventEmitter<JobSearchParams>();

  inputJobTitle: string | null = null;
  filteredJobs: string[] = [];

  inputCountryName: Country | null = null;
  filteredCountries: Country[] = [];

  ngOnInit() {
    this.filteredJobs = [...JOB_SUGGESTIONS];
    this.filteredCountries = [...COUNTRIES];
  }

  filterJobs(event: AutoCompleteCompleteEvent) {
    const query = (event.query ?? '').toLowerCase();

    this.filteredJobs = JOB_SUGGESTIONS.filter((job) => job.toLowerCase().includes(query));
  }

  filterCountries(event: AutoCompleteCompleteEvent) {
    const query = (event.query ?? '').toLowerCase();

    this.filteredCountries = COUNTRIES.filter((country) =>
      country.name.toLowerCase().includes(query),
    );
  }

  onSearch() {
    const params: JobSearchParams = {
      jobTitle: this.inputJobTitle ?? undefined,
      country: this.inputCountryName?.name,
    };
    this.search.emit(params);
  }

  onFilter() {}
}
