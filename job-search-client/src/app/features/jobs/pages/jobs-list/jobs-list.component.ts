import { Component, inject, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { JobItemComponent } from '../../components/item/job-item.component';
import { JobsService } from '../../services/jobs.service';
import { FormsModule } from '@angular/forms';
import { AutoComplete } from 'primeng/autocomplete';
import { FloatLabel } from 'primeng/floatlabel';
import { Paginator, PaginatorState } from 'primeng/paginator';
import { Job } from '../../components/item/job.model';

interface Country {
  name: string;
  code: string;
}

@Component({
  selector: 'app-jobs-list',
  imports: [
    CommonModule,
    ButtonModule,
    JobItemComponent,
    FormsModule,
    AutoComplete,
    FloatLabel,
    Paginator,
  ],
  templateUrl: './jobs-list.component.html',
  styleUrl: './jobs-list.component.scss',
})
export class JobsListComponent implements OnInit {
  private _jobsService = inject(JobsService);

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

  jobs: Job[] = [];
  pagedJobs: Job[] = [];

  value: string | null = null;
  items: string[] = [];

  selectedCountry: Country | null = null;
  filteredCountries: Country[] = [];

  page = 0;
  pageSize = 10;
  totalRecords = 0;

  ngOnInit(): void {
    this._jobsService.getJobs().subscribe((jobs) => {
      this.jobs = jobs;
      this.totalRecords = jobs.length;
      this.updatePagedJobs();
    });
  }

  updatePagedJobs() {
    const start = this.page * this.pageSize;
    const end = start + this.pageSize;
    this.pagedJobs = this.jobs.slice(start, end);
  }

  onPageChange(event: PaginatorState) {
    this.page = event.page ?? 0;
    this.pageSize = event.rows ?? this.pageSize;
    this.updatePagedJobs();
  }
}
