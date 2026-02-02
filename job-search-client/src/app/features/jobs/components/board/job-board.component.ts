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
import { finalize, map, Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import {toSignal} from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-job-board',
  imports: [ButtonModule, JobCardComponent, FormsModule, Paginator],
  templateUrl: './job-board.component.html',
  styleUrl: './job-board.component.scss',
})
// cache search results, page
export class JobBoardComponent {
  private _router = inject(Router);
  private _route = inject(ActivatedRoute);
  private _jobsService = inject(JobService);
  private _pageFromUrl = toSignal(
    this._route.queryParamMap.pipe(map(params => params.get('page')))
  );

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
      const urlPage = this._pageFromUrl();
      const currentSearch = this.searchParams();

      // Update local state from URL (prioritizes address bar over default)
      this.pageNumber = urlPage ? Number(urlPage) : 1;

      if (this.pageNumber < 1) {
        this.updateUrl(1);
        return;
      }

      // Only fetch if we have valid params or it's the initial load
      this.loadJobs();
    });
  }

  onPageChange(event: PaginatorState) {
    const newPage = event.page ? event.page + 1 : 1;
    //this.pageSize = event.rows ?? this.pageSize;

    this._router.navigate([], {
      relativeTo: this._route,
      queryParams: { page: newPage },
      queryParamsHandling: 'merge',
    });

    this.scrollToTop();
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

          if (res.pageNumber !== this.pageNumber) {
            this.updateUrl(res.pageNumber);
          }
        },
        error: () => this.updateUrl(1)
      });
  }

  private updateUrl(page: number) {
    this._router.navigate([], {
      relativeTo: this._route,
      queryParams: { page: page },
      queryParamsHandling: 'merge',
      replaceUrl: true // Critical: Replaces the 'bad' URL so the 'Back' button works
    });
  }

  get skeletonArray() {
    return Array(this.pageSize).fill(0);
  }

  scrollToTop() {
    if (this.scrollTarget) {
      this.scrollTarget.nativeElement.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }
}
