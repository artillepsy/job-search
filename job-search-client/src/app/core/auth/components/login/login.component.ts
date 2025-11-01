import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-login',
  imports: [
    ButtonModule,
    FormsModule,
],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  startUsername?: string = 'default user';
  username?: string;
  password?: string;

  onLoginChange(username: string) {
    this.username = username;
    console.log(`username: ${username}`);
  }

  submitData() {
    console.log(`submit. Username: ${this.username}, password: ${this.password}`);
    
  }
}
