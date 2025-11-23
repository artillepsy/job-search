import { Component, effect, inject, input, Input, OnInit, Signal } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { JobItemComponent } from '../item/job-item.component';
import { JobsService } from '../../services/jobs.service';
import { FormsModule } from '@angular/forms';
import { Paginator, PaginatorState } from 'primeng/paginator';
import { Job } from '../../models/job.model';
import { JobSearchParams } from '../../models/job-search.params.model';

@Component({
  selector: 'app-job-results',
  imports: [CommonModule, ButtonModule, JobItemComponent, FormsModule, Paginator],
  templateUrl: './job-results.component.html',
  styleUrl: './job-results.component.scss',
})
export class JobResultsComponent implements OnInit {
  private _jobsService = inject(JobsService);

  searchParams = input<JobSearchParams>({
    JobTitle: '',
    Country: '',
  });

  jobs: Job[] = [];
  pagedJobs: Job[] = [];

  page = 0;
  pageSize = 20;
  totalRecords = 0;

  private readonly searchEffect = effect(() => {
    this.onSearch(this.searchParams());
  });

  ngOnInit() {
    this._jobsService.getAllJobs().subscribe((jobs) => {
      this.jobs = jobs;
      this.totalRecords = jobs.length;
      this.updatePagedJobs();
    });
  }

  //todo: apply filters
  onSearch(params: JobSearchParams) {
    if (!params.JobTitle) {
      //todo: don't search if no input
      this._jobsService.getAllJobs().subscribe((jobs) => {
        this.jobs = jobs;
        this.totalRecords = jobs.length;
        this.updatePagedJobs();
      });
      return;
    }

    this._jobsService.getJobsByTitle(params.JobTitle).subscribe((jobs) => {
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
