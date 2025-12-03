import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobItemsComponent as JobsListComponent } from './job-items.component';

describe('JobItemsComponent', () => {
  let component: JobsListComponent;
  let fixture: ComponentFixture<JobsListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JobsListComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(JobsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
