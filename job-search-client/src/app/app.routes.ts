import { Routes } from '@angular/router';
import { authGuard } from './core/auth/guards/auth-guard';
import { JobBoardComponent } from './features/jobs/components/board/job-board.component';
import { LoginComponent } from './core/auth/components/login/login.component';
import { JobPageComponent } from './features/jobs/components/page/job-page.component';

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
    component: JobPageComponent,
  },

  {
    path: '**', // declare at the very bottom
    redirectTo: '/login',
  },
];
