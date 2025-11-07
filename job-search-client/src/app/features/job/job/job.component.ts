import { Component } from '@angular/core';
import { ButtonModule } from "primeng/button";

@Component({
  selector: 'app-job',
  imports: [ButtonModule],
  templateUrl: './job.component.html',
  styleUrl: './job.component.scss',
})
export class JobComponent {
  public title: string = "Senior Software Engineer";
  public companyName: string = "Tech Innovations Inc."
  public description: string = "Join our dynamic team to develop cutting-edge software solutions.";

  public apply(): void {
    alert(`Thank you for your interest in the ${this.title} position at ${this.companyName}. Our HR team will contact you soon!`);
  }
}
