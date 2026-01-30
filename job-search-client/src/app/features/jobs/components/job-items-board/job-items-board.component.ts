import { Component, effect, ElementRef, inject, input, OnInit, ViewChild } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { JobItemComponent } from '../job-item/job-item.component';
import { JobResponse, JobsService } from '../../services/jobs.service';
import { FormsModule } from '@angular/forms';
import { Paginator, PaginatorState } from 'primeng/paginator';
import { Job } from '../../models/job.model';
import { JobSearchParams } from '../../models/job-search.params.model';

@Component({
  selector: 'app-job-items-board',
  imports: [ButtonModule, JobItemComponent, FormsModule, Paginator],
  templateUrl: './job-items-board.component.html',
  styleUrl: './job-items-board.component.scss',
})
// cache search results, page
export class JobItemsBoardComponent implements OnInit {
  private _jobsService = inject(JobsService);

  // input from parent
  searchParams = input<JobSearchParams | undefined>(undefined);

  @ViewChild('scrollTarget') scrollTarget!: ElementRef;

  jobs: Job[] = [];

  totalPages = 0;
  pageNumber = 1;
  pageSize = 23;
  totalRecords = 0;

  constructor() {
    effect(() => {
      if (!this.searchParams()) {
        return;
      }

      this.loadJobs();
    });
  }

  ngOnInit() {
    this.loadJobs();
  }

  onPageChange(event: PaginatorState) {
    this.pageNumber = event.page ? event.page + 1 : 1; // page + 1
    this.pageSize = event.rows ?? this.pageSize;

    if (this.scrollTarget) {
      this.scrollTarget.nativeElement.scrollTo({
        top: 0,
        behavior: 'smooth', // Optional: removes the 'jump' and slides up
      });
    }

    this.loadJobs();
  }

  // filters as params
  loadJobs() {
    const params: JobSearchParams = {
      ...this.searchParams(),
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
    };

    this._jobsService.getJobs(params).subscribe((res: JobResponse) => {
      this.jobs = res.jobs;
      this.totalRecords = res.totalRecords;
      this.totalPages = res.totalPages;
    });
  }
}
