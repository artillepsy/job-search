import { Component, computed, ElementRef, inject, input, signal, ViewChild } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { JobCardComponent } from '../card/job-card.component';
import { JobService } from '../../services/job.service';
import { FormsModule } from '@angular/forms';
import { Paginator, PaginatorState } from 'primeng/paginator';
import { JobSearchParams } from '../../models/job-search-params.model';
import { catchError, combineLatest, finalize, map, of, startWith, switchMap, tap } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-job-board',
  imports: [ButtonModule, JobCardComponent, FormsModule, Paginator],
  templateUrl: './job-board.component.html',
  styleUrl: './job-board.component.scss',
})
// cache search results, page
export class JobBoardComponent {
  private readonly _router = inject(Router);
  private readonly _route = inject(ActivatedRoute);
  private readonly _jobService = inject(JobService);

  readonly PAGE_SIZE = 19;
  readonly skeletonArray = Array(this.PAGE_SIZE).fill(0);

  @ViewChild('scrollTarget') scrollTarget!: ElementRef;

  searchParams = input<JobSearchParams | undefined>(undefined);

  private _search$ = toObservable(this.searchParams);
  private readonly _page$ = this._route.queryParamMap.pipe(
    map(params => Number(params.get('page')) || 1)
  );

  private readonly _jobResource$ = combineLatest([this._search$, this._page$]).pipe(
    switchMap(([search, page]) => {
      const params = { ...search, pageNumber: page, pageSize: this.PAGE_SIZE };
      return this._jobService.getJobs(params).pipe(
        catchError(() => of({ jobs: [], totalRecords: 0, pageNumber: 1 }))
      );
    })
  );

  readonly jobsResponse = toSignal(this._jobResource$, {
    initialValue: { jobs: [], totalRecords: 0, pageNumber: 1 }
  });
  private readonly _urlPage = toSignal(this._page$, { initialValue: 1 });

  readonly jobs = computed(() => this.jobsResponse()?.jobs ?? []);
  readonly totalRecords = computed(() => this.jobsResponse()?.totalRecords ?? 0);
  readonly isLoading = computed(() => !this.jobsResponse());
  readonly currentPage = computed(() => {
    return this.jobsResponse()?.pageNumber ?? this._urlPage();
  });

  onPageChange(event: PaginatorState) {
    const newPage = (event.page ?? 0) + 1;
    this._router.navigate([], {
      relativeTo: this._route,
      queryParams: { page: newPage },
      queryParamsHandling: 'merge',
    });
    this.scrollToTop();
  }

  scrollToTop() {
    if (this.scrollTarget) {
      this.scrollTarget.nativeElement.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }
}
