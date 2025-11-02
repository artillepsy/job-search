import { Component, inject } from '@angular/core';
import { LoginService } from '../../../../core/auth/services/login.service';

@Component({
  selector: 'app-dummy',
  imports: [],
  templateUrl: './dummy.component.html',
  styleUrl: './dummy.component.scss',
})
export class DummyComponent {
  loginService = inject(LoginService);
  
}
