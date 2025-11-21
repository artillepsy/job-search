import { Routes } from '@angular/router';
import { authGuard } from './core/auth/guards/auth-guard';
import { JobResultsComponent } from './features/jobs/components/job-results/job-results.component';
import { LoginComponent } from './core/auth/components/login/login.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'jobs',
    pathMatch: 'full',
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  /*{
    path: 'welcome',
    component: DummyComponent,
    canActivate: [authGuard]
},*/
  {
    path: 'jobs',
    component: JobResultsComponent,
  },

  {
    path: '**', // declare at the very bottom
    redirectTo: '/login',
  },
];
