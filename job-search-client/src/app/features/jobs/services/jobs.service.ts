import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Job } from '../models/job.model';

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

  getAllJobs(pageNumber: number, pageSize: number): Observable<JobResponse> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this._http.get<JobResponse>('/api/jobs/get-all', { params });
  }

  getJobsByTitle(title: string): Observable<Job[]> {
    const params = new HttpParams().set('title', title);
    return this._http.get<Job[]>('/api/jobs/get', { params });
  }
}
