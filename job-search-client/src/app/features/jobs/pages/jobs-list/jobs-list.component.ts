import { Component, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { Job } from '../../components/item/job.model';
import { JobItemComponent } from '../../components/item/job-item.component';

@Component({
  selector: 'app-jobs-list',
  imports: [CommonModule, ButtonModule, JobItemComponent],
  templateUrl: './jobs-list.component.html',
  styleUrl: './jobs-list.component.scss',
})
export class JobsListComponent implements OnInit {
  jobs: Job[] = [];

  ngOnInit(): void {
    console.log('init');
  }

  onClickLoadJobs() {
    console.log('load jobs');

    this.jobs = [
      {
        id: 1,
        title: 'Senior Software Engineer',
        companyName: 'Tech Innovations Inc.',
        description: 'Build features, review code, own services.',
      },
      {
        id: 2,
        title: 'Frontend Developer',
        companyName: 'Pixel Works',
        description: 'Angular, accessibility, performance.',
      },
      {
        id: 3,
        title: 'Backend Developer',
        companyName: 'Tech Solutions',
        description: 'Node.js, SQL, NoSQL.',
      },
      {
        id: 4,
        title: 'DevOps Engineer',
        companyName: 'Cloud Solutions',
        description: 'Docker, Kubernetes, CI/CD.',
      },
      {
        id: 5,
        title: 'Data Scientist',
        companyName: 'Analytics Pro',
        description: 'Machine learning, data analysis, Python.',
      },
    ];
  }
}
