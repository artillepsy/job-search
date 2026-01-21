import { Component, computed, input, Input } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { Job } from '../../models/job.model';

@Component({
  selector: 'app-job-item',
  imports: [ButtonModule],
  templateUrl: './job-item.component.html',
  styleUrl: './job-item.component.scss',
})
export class JobItemComponent {
  job = input.required<Job>();

  formattedSalary = computed(() => {
    const {salaryMin, salaryMax, currency} = this.job();
    if (salaryMin) { // salary is visible
      if (!salaryMax || (salaryMin === salaryMax)) {
        return `${salaryMin} ${currency}`;
      } else {
        return `${salaryMin}-${salaryMax} ${currency}`;
      }
    }
    return 'Salary hidden'
  });

  public apply(): void {
    //alert(`Thank you for your interest in the ${this.job.title} position at ${this.job.companyName}.
    //Our HR team will contact you soon!`);
  }

  isPostedToday() {
    return this.getPostingDays() === 0;
  }

  getPostingDays() {
    const today = new Date();
    const createdAt = new Date(this.job().createdAt);
    const timeDiff = today.getTime() - createdAt.getTime();
    return Math.floor(timeDiff / (1000 * 3600 * 24));
  }
}
