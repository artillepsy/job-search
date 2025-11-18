import { Component, inject, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { JobItemComponent } from '../../components/item/job-item.component';
import { JobsService } from '../../services/jobs.service';
import { FormsModule } from '@angular/forms';
import { AutoComplete } from 'primeng/autocomplete';
import { FloatLabel } from 'primeng/floatlabel';

interface Country {
  name: string;
  code: string;
}

@Component({
  selector: 'app-jobs-list',
  imports: [CommonModule, ButtonModule, JobItemComponent, FormsModule, AutoComplete, FloatLabel],
  templateUrl: './jobs-list.component.html',
  styleUrl: './jobs-list.component.scss',
})
export class JobsListComponent implements OnInit {
  private _jobsService = inject(JobsService);

  jobs$ = this._jobsService.jobs$;

  private _jobSuggestions: string[] = [
    'Frontend Developer',
    'Backend Developer',
    'Fullstack Developer',
    '.NET Developer',
    'QA Engineer',
    'DevOps Engineer',
  ];

  private _countries: Country[] = [
    { name: 'Poland', code: 'PL' },
    { name: 'Germany', code: 'DE' },
    { name: 'France', code: 'FR' },
    { name: 'United Kingdom', code: 'GB' },
    { name: 'United States', code: 'US' },
  ];

  value: string | null = null;
  items: string[] = [];

  selectedCountry: Country | null = null;
  filteredCountries: Country[] = [];

  ngOnInit(): void {
    console.log('init');
    this._jobsService.loadJobs();
  }

  onClickLoadJobs() {
    console.log('load jobs');
    this._jobsService.loadJobs();
  }
}
