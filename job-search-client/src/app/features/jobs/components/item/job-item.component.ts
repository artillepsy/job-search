import { Component, Input } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { Job } from '../../models/job.model';

@Component({
  selector: 'app-job-item',
  imports: [ButtonModule],
  templateUrl: './job-item.component.html',
  styleUrl: './job-item.component.scss',
})
export class JobItemComponent {
  @Input({ required: true }) job!: Job;

  public apply(): void {
    alert(`Thank you for your interest in the ${this.job.title} position at ${this.job.companyName}.
    Our HR team will contact you soon!`);
  }
}
