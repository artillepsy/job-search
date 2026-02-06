import { Component, computed, ElementRef, inject, input, ViewChild } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { JobCardComponent } from '../card/job-card.component';
import { FormsModule } from '@angular/forms';
import { Paginator, PaginatorState } from 'primeng/paginator';
import { JobBoardStateService } from '../../services/state/job-board-state.service';
import { JobUrlService } from '../../services/url/job-url.service';
import { JobFiltersComponent } from '../filters/job-filters.component';

@Component({
  selector: 'app-job-board',
  imports: [ButtonModule, JobCardComponent, FormsModule, Paginator, JobFiltersComponent],
  templateUrl: './job-board.component.html',
  styleUrl: './job-board.component.scss',
})
export class JobBoardComponent {
  private readonly _stateService = inject(JobBoardStateService);
  private readonly _urlService = inject(JobUrlService);

  @ViewChild('scrollTarget') scrollTarget!: ElementRef;

  readonly state = this._stateService.state;
  readonly pageSize = computed(() => this._urlService.params().pageSize);
  readonly skeletonArray = Array(this.pageSize()).fill(0);

  readonly currentPageIndex = computed(() => {
    const page = this._urlService.params().page;
    return page > 0 ? page - 1 : 0;
  });

  filtersVisible = input<boolean>(true);

  onPageChange(event: PaginatorState) {
    this._urlService.updateSearch({ page: (event.page ?? 0) + 1 });
    this.scrollToTop();
  }

  scrollToTop() {
    if (this.scrollTarget) {
      this.scrollTarget.nativeElement.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }
}
