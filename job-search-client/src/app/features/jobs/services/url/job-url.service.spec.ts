import { TestBed } from '@angular/core/testing';

import { JobUrlService } from './job-url.service';

describe('JobUrlService', () => {
  let service: JobUrlService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(JobUrlService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
