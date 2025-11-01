import { Routes } from '@angular/router';
import { LoginComponent } from './core/auth/components/login/login.component';

export const routes: Routes = [{
    path: 'login',
    component: LoginComponent,
},


{
    path: '**', // declare at the very bottom
    redirectTo: '/login'
},
];
