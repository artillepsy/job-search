import { Component, effect, ElementRef, inject, input, OnInit, ViewChild } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { JobItemComponent } from '../job-item/job-item.component';
import {JobResponse, JobsService} from '../../services/jobs.service';
import { FormsModule } from '@angular/forms';
import { Paginator, PaginatorState } from 'primeng/paginator';
import { Job } from '../../models/job.model';
import { JobSearchParams } from '../../models/job-search.params.model';
import { ScrollPanel } from 'primeng/scrollpanel';

@Component({
  selector: 'app-job-items-board',
  imports: [ButtonModule, JobItemComponent, FormsModule, Paginator, ScrollPanel],
  templateUrl: './job-items-board.component.html',
  styleUrl: './job-items-board.component.scss',
})
//cache search results, page
export class JobItemsBoardComponent implements OnInit {
  private _jobsService = inject(JobsService);

  searchParams = input<JobSearchParams | undefined>(undefined);

  jobs: Job[] = [];

  totalPages = 0;
  pageNumber = 0;
  pageSize = 23;
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
    this._jobsService.getAllJobs(1, this.pageSize).subscribe((data: JobResponse) => {
      this.jobs = data.jobs;
      this.totalRecords = data.totalRecords;
      this.totalPages = data.totalPages;
    });
  }

  //todo: apply filters
  onSearch(params: JobSearchParams) {
    /*if (!params.jobTitle) {
      //todo: don't search if no input
      this.loadJobs();
      return;
    }*/

    this.loadJobs(); // todo: use different method instead
    /*this._jobsService.getJobsByTitle(params.jobTitle).subscribe((jobs) => {
      this.jobs = jobs;
      this.totalRecords = jobs.length;
      this.updatePagedJobs();
    });*/
  }

  // change everything to filtered search. To get all, unnecessary fields should remain empty
  onPageChange(event: PaginatorState) {
    this.pageNumber = event.page ? event.page + 1 : 1; // page + 1
    this.pageSize = event.rows ?? this.pageSize;

    this.loadJobs();
  }

  // filters as params
  loadJobs() {
    this._jobsService.getAllJobs(this.pageNumber, this.pageSize).subscribe((res: JobResponse) => {
      this.jobs = res.jobs;
      this.totalRecords = res.totalRecords;
    });
  }
}
