import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Job } from '../models/job.model';
import { environment } from '../../../../environments/environment';
import { JobSearchParams } from '../models/job-search.params.model';

export interface JobResponse {
  totalPages: number;
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  returnRecords: number;
  jobs: Job[];
}

@Injectable({
  providedIn: 'root',
})
export class JobsService {
  private _http = inject(HttpClient);

  getJobs(params: JobSearchParams): Observable<JobResponse> {
    let httpParams = new HttpParams();

    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        httpParams = httpParams.append(key, value.toString());
      }
    });
    return this._http.get<JobResponse>(`${environment.apiUrl}/jobs/get`, { params: httpParams });
  }
}
