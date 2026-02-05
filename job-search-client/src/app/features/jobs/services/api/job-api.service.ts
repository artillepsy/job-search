import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { JobSearchParams } from '../../models/job-search-params.model';
import { JobCardData } from '../../models/job-card-data.model';

export interface JobApiResponse {
  totalPages: number;
  page: number;
  pageSize: number;
  totalRecords: number;
  returnRecords: number;
  jobs: JobCardData[];
}

@Injectable({
  providedIn: 'root',
})
export class JobApiService {
  private _http = inject(HttpClient);

  getJobs(params: JobSearchParams): Observable<JobApiResponse> {
    let httpParams = new HttpParams();

    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        httpParams = httpParams.append(key, value.toString());
      }
    });
    return this._http.get<JobApiResponse>(`${environment.apiUrl}/jobs/get`, { params: httpParams });
  }
}
