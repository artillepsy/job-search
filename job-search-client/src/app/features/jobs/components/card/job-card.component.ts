import { Component, computed, input, Input } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { WEBSITE_SOURCE_CONFIG } from '../../../../core/constants/storage.constants';
import { JobCardData } from '../../models/job-card-data.model';

@Component({
  selector: 'app-job-card',
  imports: [ButtonModule],
  templateUrl: './job-card.component.html',
  styleUrl: './job-card.component.scss',
})
export class JobCardComponent {
  jobInfo = input.required<JobCardData>();

  isSalaryVisible = computed(() => this.jobInfo().salaryMin !== null);

  formattedSalary = computed(() => {
    const { salaryMin, salaryMax, currency } = this.jobInfo();
    if (salaryMin) {
      // salary is visible
      if (!salaryMax || salaryMin === salaryMax) {
        return `${salaryMin} ${currency}`;
      } else {
        return `${salaryMin} - ${salaryMax} ${currency}`;
      }
    }
    return 'Salary negotiable';
  });

  daysAgoText = computed(() => {
    let rawDate = this.jobInfo().createdAt;

    const today = new Date();
    today.setHours(0, 0, 0, 0); // Reset time to start of day for accurate day counting

    const createdDateOnly = new Date(rawDate);
    createdDateOnly.setHours(0, 0, 0, 0);

    const timeDiff = today.getTime() - createdDateOnly.getTime();
    const days = Math.floor(timeDiff / (1000 * 3600 * 24));

    // 3. Return the formatted string
    if (days === 0) return 'Today';
    if (days === 1) return 'Yesterday';
    return `${days} days ago`;
  });

  srcMetadata = computed(() => {
    const sourceKey = this.jobInfo().website; // This is the string like 'CareersInPoland'
    return WEBSITE_SOURCE_CONFIG[sourceKey] || null;
  });

  public apply(): void {
    window.open(this.jobInfo().url, '_blank');
  }
}
