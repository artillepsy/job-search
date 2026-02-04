import { TestBed } from '@angular/core/testing';

import { JobBoardStateService } from './job-board-state.service';

describe('JobBoardStateService', () => {
  let service: JobBoardStateService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(JobBoardStateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
