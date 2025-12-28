import { Component, effect, inject, input, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { JobItemComponent } from '../item/job-item.component';
import {JobResponse, JobsService} from '../../services/jobs.service';
import { FormsModule } from '@angular/forms';
import { Paginator, PaginatorState } from 'primeng/paginator';
import { Job } from '../../models/job.model';
import { JobSearchParams } from '../../models/job-search.params.model';

@Component({
  selector: 'app-job-items',
  imports: [ButtonModule, JobItemComponent, FormsModule, Paginator],
  templateUrl: './job-items.component.html',
  styleUrl: './job-items.component.scss',
})
//cache search results, page
export class JobItemsComponent implements OnInit {
  private _jobsService = inject(JobsService);

  searchParams = input<JobSearchParams | undefined>(undefined);

  jobs: Job[] = [];

  totalPages = 0;
  pageNumber = 0;
  pageSize = 100;
  totalRecords = 0;

  constructor() {
    effect(() => {
      if (!this.searchParams()) {
        return;
      }

      this.onSearch(this.searchParams()!);
    });
  }

  ngOnInit() {
    this._jobsService.getAllJobs(1, 100).subscribe((data: JobResponse) => {
      this.jobs = data.jobs;
      this.totalRecords = data.totalRecords;
      this.totalPages = data.totalPages;
    });
  }

  //todo: apply filters
  onSearch(params: JobSearchParams) {
    if (!params.jobTitle) {
      //todo: don't search if no input
      this.loadJobs();
      return;
    }

    this.loadJobs(); // todo: use different method instead
    /*this._jobsService.getJobsByTitle(params.jobTitle).subscribe((jobs) => {
      this.jobs = jobs;
      this.totalRecords = jobs.length;
      this.updatePagedJobs();
    });*/
  }

  onPageChange(event: PaginatorState) {
    this.pageNumber = event.page ? event.page + 1 : 1; // page + 1
    this.pageSize = event.rows ?? this.pageSize;

    this.loadJobs();
  }

  // filters as params
  loadJobs() {
    this._jobsService.getAllJobs(this.pageNumber, this.pageSize)
      .subscribe((res: JobResponse) => {
        this.jobs = res.jobs;
        this.totalRecords = res.totalRecords;
      });
  }
}
