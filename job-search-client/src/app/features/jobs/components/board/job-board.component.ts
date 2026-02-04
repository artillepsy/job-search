import { Component, computed, effect, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { JobCardComponent } from '../card/job-card.component';
import { FormsModule } from '@angular/forms';
import { Paginator, PaginatorState } from 'primeng/paginator';
import { combineLatest, filter, map, skip, switchMap } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { JobBoardStateService } from '../../services/state/job-board-state.service';
import { JobUrlService } from '../../services/url/job-url.service';

@Component({
  selector: 'app-job-board',
  imports: [ButtonModule, JobCardComponent, FormsModule, Paginator],
  templateUrl: './job-board.component.html',
  styleUrl: './job-board.component.scss',
})
// cache search results, page
export class JobBoardComponent {
  private readonly _router = inject(Router);
  private readonly _route = inject(ActivatedRoute);
  private readonly _stateService = inject(JobBoardStateService);
  private readonly _urlService = inject(JobUrlService);

  @ViewChild('scrollTarget') scrollTarget!: ElementRef;

  constructor() {
    effect(() => {
      console.log(`Page size changed: ${this.pageSize()}`);
      console.log(`Total records: ${this.state().totalRecords}`);
    })
  }

  readonly state = this._stateService.state;
  readonly pageSize = computed(() => this._urlService.params().pageSize);
  readonly skeletonArray = Array(this.pageSize()).fill(0);

  readonly currentPageIndex = computed(() => {
    const page = this._urlService.params().pageNumber;
    return page > 0 ? page - 1 : 0;
  });

  onPageChange(event: PaginatorState) {
    this._urlService.updateSearch({ pageNumber: (event.page ?? 0) + 1 });
    this.scrollToTop();
  }

  scrollToTop() {
    if (this.scrollTarget) {
      this.scrollTarget.nativeElement.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }
}
