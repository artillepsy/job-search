import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FiltersDropdownComponent } from './filters-dropdown.component';

describe('FiltersDropdownComponent', () => {
  let component: FiltersDropdownComponent;
  let fixture: ComponentFixture<FiltersDropdownComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FiltersDropdownComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(FiltersDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
