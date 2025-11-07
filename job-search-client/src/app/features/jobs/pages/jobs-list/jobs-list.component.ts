import { Component, OnInit } from '@angular/core';
import { JobItemComponent } from '../../components/item/job-item.component';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-jobs-list',
  imports: [JobItemComponent, ButtonModule],
  templateUrl: './jobs-list.component.html',
  styleUrl: './jobs-list.component.scss',
})
export class JobsListComponent implements OnInit {
  ngOnInit(): void {
    console.log('init');
  }

  onClickLoadJobs() {
    console.log('load jobs');
  }
}
