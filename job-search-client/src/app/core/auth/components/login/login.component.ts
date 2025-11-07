import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { LoginService } from '../../services/login.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [ButtonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  readonly startUsername = 'default user';
  username = this.startUsername;
  password?: string;

  private loginService = inject(LoginService);
  private router = inject(Router);

  onLoginChange(username: string) {
    this.username = username;
    console.log(`username: ${username}`);
  }

  submitData() {
    console.log(`submit. Username: ${this.username}, password: ${this.password}`);

    if (!this.password) {
      console.error('null password!');
      return;
    }
    if (this.loginService.logIn(this.username, this.password)) {
      this.router.navigate(['/welcome']);
    } else {
      alert('Invalid username or password');
    }
  }
}
