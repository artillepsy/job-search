import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobSearchPanelComponent } from './job-search-panel.component';

describe('JobSearchPanelComponent', () => {
  let component: JobSearchPanelComponent;
  let fixture: ComponentFixture<JobSearchPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JobSearchPanelComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(JobSearchPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
