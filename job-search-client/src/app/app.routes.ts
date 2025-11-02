import { Routes } from '@angular/router';
import { LoginComponent } from './core/auth/components/login/login.component';
import { DummyComponent } from './features/dummy/components/dummy/dummy.component';
import { authGuard } from './core/auth/guards/auth-guard';

export const routes: Routes = [{
    path: 'login',
    component: LoginComponent,
},
{
    path: 'welcome',
    component: DummyComponent,
    canActivate: [authGuard]
},


{
    path: '**', // declare at the very bottom
    redirectTo: '/login'
},
];
