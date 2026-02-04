import { computed, inject, Injectable, signal } from '@angular/core';
import { JobCardData } from '../../models/job-card-data.model';
import { JobApiService } from '../api/job-api.service';
import { JobSearchParams } from '../../models/job-search-params.model';
import { catchError, finalize, of, tap } from 'rxjs';

export interface JobBoardState {
  jobs: JobCardData[];
  totalRecords: number;
  totalPages: number;
  currentPage: number;
  isLoading: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class JobBoardStateService {
  private readonly _jobApiService = inject(JobApiService);

  private readonly _state = signal<JobBoardState>({
    jobs: [],
    totalRecords: 0, // total jobs on all pages
    totalPages: 0,
    isLoading: true,
    currentPage: 1,
  });

  public readonly state = this._state.asReadonly();

  fetchJobs(
    searchParams: JobSearchParams | undefined,
    pageNumber: number,
    pageSize: number)
  {
    this._state.update((state) => ({ ...state, isLoading: true }));

    const params = { ...searchParams, pageNumber: pageNumber, pageSize: pageSize };

    return this._jobApiService.getJobs(params).pipe(
      tap((response) =>
        this._state.update(s => ({
          ...s,
          jobs: response.jobs,
          totalRecords: response.totalRecords,
          totalPages: response.totalPages,
          currentPage: response.pageNumber,
          isLoading: false,
        })),
      ),
      catchError(() => {
        this._state.update(s => ({ ...s, jobs: [], totalRecords: 0, isLoading: false }));
        return of(null);
      }),
    );
  }
}
