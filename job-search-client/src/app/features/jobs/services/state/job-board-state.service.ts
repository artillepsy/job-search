import { inject, Injectable, signal } from '@angular/core';
import { JobCardData } from '../../models/job-card-data.model';
import { JobApiService } from '../api/job-api.service';
import {
  catchError,
  distinctUntilChanged,
  EMPTY,
  filter,
  finalize,
  of,
  switchMap,
  tap,
} from 'rxjs';
import {JobUrlService} from '../url/job-url.service';
import {toObservable} from '@angular/core/rxjs-interop';

export interface JobBoardState {
  jobs: JobCardData[];
  totalRecords: number;
  totalPages: number;
  currentPage: number;
  isLoading: boolean;
  error: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class JobBoardStateService {
  private readonly _apiService = inject(JobApiService);
  private readonly _urlService = inject(JobUrlService);

  private readonly _state = signal<JobBoardState>({
    jobs: [],
    totalRecords: 0, // total jobs on all pages
    totalPages: 0,
    isLoading: true,
    currentPage: 1,
    error: null,
  });

  public readonly state = this._state.asReadonly();

  constructor() {
    toObservable(this._urlService.params)
      .pipe(
        filter(() => !this._urlService.shouldSkipFetch()),
        distinctUntilChanged((prev, curr) => JSON.stringify(prev) === JSON.stringify(curr)),
        tap(() => this._state.update((s) => ({ ...s, isLoading: true }))),
        switchMap((params) =>
          this._apiService.getJobs(params).pipe(
            catchError((err) => {
              this.handleError();
              return EMPTY;
            }),
          ),
        ),
      )
      .subscribe((response) => {
        const maxPage = response.totalPages || 1;
        const requestedPage = this._urlService.params().pageNumber || 1;

        console.log(`total pages: ${response.totalPages} of ${maxPage} loaded.`);

        if (requestedPage < 1 || requestedPage > maxPage) {
          // Pass 'true' to set the flag before the next signal emission
          this._urlService.updateSearch({ pageNumber: response.pageNumber }, true);
        }

        this._state.set({
          jobs: response.jobs,
          totalRecords: response.totalRecords,
          totalPages: response.totalPages,
          currentPage: response.pageNumber,
          isLoading: false,
          error: null,
        });
      });
  }

  private handleError() {
    this._state.update(s => ({
      ...s,
      jobs: [],
      totalRecords: 0,
      isLoading: false,
      error: 'Failed to load jobs. Please try again.',
    }));
  }

}
