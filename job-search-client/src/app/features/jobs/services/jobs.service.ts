import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';

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
  private _jobs$ = new BehaviorSubject<JobResponse[]>([]);
  private _http = inject(HttpClient);

  jobs$ = this._jobs$.asObservable();

  loadJobs() {
    this._http.get<JobResponse[]>('/api/jobs/get-all').subscribe({
      next: (jobs) => this._jobs$.next(jobs),
      error: (err) => console.error('Error loading jobs', err),
    });
  }
}
