import { Routes } from '@angular/router';
import { UserComponent } from './user/user.component';
import { RegistrationComponent } from './user/registration/registration.component';
import { LoginComponent } from './user/login/login.component';
import { DashboardComponent } from './dashboard/dashboard-component';
import { authGuard, isNotLoggedInGuard } from './core/guards/auth-guard';

export const routes: Routes = [
    { path: '', redirectTo: '/signin', pathMatch: 'full' },
    {
        path: '', component: UserComponent,
        children: [
            { path: 'signup', component: RegistrationComponent, canActivate: [isNotLoggedInGuard] },
            { path: 'signin', component: LoginComponent, canActivate: [isNotLoggedInGuard] }
        ]
    },
    { 
        path: 'dashboard', component: DashboardComponent,
        canActivate: [authGuard]
    }
];
