import {
  Component,
  effect,
  ElementRef,
  inject,
  input,
  OnInit,
  signal,
  ViewChild,
} from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { JobCardComponent } from '../card/job-card.component';
import { JobService } from '../../services/job.service';
import { FormsModule } from '@angular/forms';
import { Paginator, PaginatorState } from 'primeng/paginator';
import { JobInfo } from '../../models/job-info.model';
import { JobSearchParams } from '../../models/job-search-params.model';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-job-board',
  imports: [ButtonModule, JobCardComponent, FormsModule, Paginator],
  templateUrl: './job-board.component.html',
  styleUrl: './job-board.component.scss',
})
// cache search results, page
export class JobBoardComponent implements OnInit {
  private _jobsService = inject(JobService);

  searchParams = input<JobSearchParams | undefined>(undefined);
  loading = signal<boolean>(false);

  @ViewChild('scrollTarget') scrollTarget!: ElementRef;

  jobs: JobInfo[] = [];

  totalPages = 0;
  pageNumber = 1;
  pageSize = 23;
  totalRecords = 0;

  constructor() {
    effect(() => {
      if (!this.searchParams()) {
        return;
      }

      this.pageNumber = 1;
      this.scrollToTop();
      this.loadJobs();
    });
  }

  ngOnInit() {
    this.loadJobs();
  }

  onPageChange(event: PaginatorState) {
    this.pageNumber = event.page ? event.page + 1 : 1; // page + 1
    this.pageSize = event.rows ?? this.pageSize;

    this.scrollToTop();
    this.loadJobs();
  }

  get skeletonArray() {
    return Array(this.pageSize).fill(0);
  }

  scrollToTop() {
    if (this.scrollTarget) {
      this.scrollTarget.nativeElement.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  // filters as params
  loadJobs() {
    this.loading.set(true);

    const params: JobSearchParams = {
      ...this.searchParams(),
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
    };

    this._jobsService
      .getJobs(params)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (res) => {
          this.jobs = res.jobs;
          this.totalRecords = res.totalRecords;
          this.totalPages = res.totalPages;
        },
      });
  }
}
