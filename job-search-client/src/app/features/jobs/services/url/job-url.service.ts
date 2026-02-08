import { computed, effect, inject, Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import {
  JOB_SEARCH_KEYS,
  JOB_SEARCH_KEYS_EXCLUDED_FROM_URL,
  JobSearchParams,
  PAGE_STR_KEY,
} from '../../models/job-search-params.model';

@Injectable({
  providedIn: 'root',
})
export class JobUrlService {
  private _router = inject(Router);
  private _route = inject(ActivatedRoute);
  private _skipNextFetch = false;

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

  constructor() {
    effect(() => {
      const raw = this._rawParams();
      const validKeys = JOB_SEARCH_KEYS;
      const rawKeys = Object.keys(raw);

      const hasGarbage = rawKeys.some(key => !validKeys.includes(key as any));

      if (hasGarbage) {
        // Trigger a silent update to strip the garbage immediately
        this.updateSearch({}, true);
      }
    });
  }

  updateSearch(changes: Partial<JobSearchParams>, silent: boolean = false) {
    this._skipNextFetch = silent;

    const currentState = this.params();

    const nextState = {
      ...currentState,
      ...changes,
    };

    const isFilterChange = Object.keys(changes).some((k) => k !== PAGE_STR_KEY);
    if (isFilterChange) {
      nextState.page = 1;
    }

    const cleanParams = Object.fromEntries(
      Object.entries(nextState).filter(([key, value]) => {
        if (JOB_SEARCH_KEYS_EXCLUDED_FROM_URL.includes(key as any)) {
          return false;
        }
        if (value === null || value === undefined || value === '') {
          return false;
        }
        if (value === false) {
          return false;
        }
        return true;
      }),
    );

    this._router.navigate([], {
      queryParams: cleanParams,
    });
  }

  shouldSkipFetch(): boolean {
    const skip = this._skipNextFetch;
    this._skipNextFetch = false;
    return skip;
  }
}
