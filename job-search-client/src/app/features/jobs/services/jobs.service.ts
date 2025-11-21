import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Job } from '../models/job.model';

export interface JobResponse {
  id?: number;
  title: string;
  companyName: string;
  salary: number;
}

@Injectable({
  providedIn: 'root',
})
export class JobsService {
  private _http = inject(HttpClient);

  getAllJobs(): Observable<Job[]> {
    return this._http.get<Job[]>('/api/jobs/get-all');
  }

  getJobsByTitle(title: string): Observable<Job[]> {
    const params = new HttpParams().set('title', title);
    return this._http.get<Job[]>('/api/jobs/get', { params });
  }
}
