import { Component, output } from '@angular/core';
import { AutoComplete } from 'primeng/autocomplete';
import { Button } from 'primeng/button';
import { FloatLabel } from 'primeng/floatlabel';
import { JobSearchParams } from '../../models/job-search.params.model';
import { FormsModule } from '@angular/forms';
import { InputText } from 'primeng/inputtext';

@Component({
  selector: 'app-search-panel',
  imports: [AutoComplete, Button, FloatLabel, FormsModule, InputText],
  templateUrl: './job-search-panel.component.html',
  styleUrl: './job-search-panel.component.scss',
})
export class JobSearchPanelComponent {
  searchChange = output<JobSearchParams>();
  filtersChange = output();

  inputTitle: string | null = null;

  onClickSearch() {
    const params: JobSearchParams = {
      title: this.inputTitle ?? undefined,
    };
    this.searchChange.emit(params);
  }

  onClickFilters() {
    this.filtersChange.emit();
  }
}
