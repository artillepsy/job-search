import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Job } from '../components/item/job.model';

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

  getJobs(): Observable<Job[]> {
    return this._http.get<Job[]>('/api/jobs/get-all');
  }
}
