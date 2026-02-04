import { computed, inject, Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { JOB_SEARCH_KEYS, JobSearchParams } from '../../models/job-search-params.model';

@Injectable({
  providedIn: 'root',
})
export class JobUrlService {
  private _router = inject(Router);
  private _route = inject(ActivatedRoute);

  readonly _rawParams = toSignal(this._route.queryParams, { initialValue: {} });

  readonly params = computed<JobSearchParams>(() => {
    const raw = this._rawParams();
    const template = new JobSearchParams();
    const result = {} as any;

    JOB_SEARCH_KEYS.forEach(key => {
      const rawValue = (raw as any)[key];
      const defaultValue = (template as any)[key];

      // If missing from URL, use the default (null)
      if (rawValue === undefined || rawValue === null || rawValue === '') {
        result[key] = defaultValue;
        return;
      }

      // Determine type for conversion
      // If defaultValue is null, typeof returns 'object'. We treat those as strings.
      const type = typeof defaultValue;

      if (type === 'number') result[key] = Number(rawValue);
      else if (type === 'boolean') result[key] = rawValue === 'true';
      else result[key] = rawValue;
    });

    return result as JobSearchParams;
  });

  updateSearch(changes: Partial<JobSearchParams>) {
    const isFilterChange = Object.keys(changes).some(k => k !== 'pageNumber');
    const newParams = {
      ...changes,
      pageNumber: isFilterChange ? 1 : (changes.pageNumber || 1)
    };

    const cleanParams = Object.fromEntries(
      Object.entries(newParams).filter(([_, value]) =>
        value !== null && value !== undefined && value !== ''
      )
    );

    this._router.navigate([], {
      queryParams: cleanParams, // Use the clean version
      queryParamsHandling: 'merge'
    });
  }
}
