import { inject, Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { JobSearchParams } from '../../models/job-search-params.model';

@Injectable({
  providedIn: 'root',
})
export class JobUrlService {
  private _router = inject(Router);
  private _route = inject(ActivatedRoute);

  readonly params = toSignal(this._route.queryParams, { initialValue: {} });

  updateSearch(changes: Partial<JobSearchParams>) {
    const keys = Object.keys(changes);
    const isOnlyPageChange = keys.length === 1 && keys[0] === 'pageNumber';

    this._router.navigate([], {
      queryParams: {
        ...changes,
        pageNumber: isOnlyPageChange ? (changes.pageNumber || 1) : 1
      },
      queryParamsHandling: 'merge'
    });
  }
}
