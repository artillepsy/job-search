import { Component } from '@angular/core';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-item-item',
  imports: [ButtonModule],
  templateUrl: './job-item.component.html',
  styleUrl: './job-item.component.scss',
})
export class JobItemComponent {
  public title: string = 'Senior Software Engineer';
  public companyName: string = 'Tech Innovations Inc.';
  public description: string = 'Join our dynamic team to develop cutting-edge software solutions.';

  public apply(): void {
    alert(`Thank you for your interest in the ${this.title} position at ${this.companyName}.
    Our HR team will contact you soon!`);
  }
}
