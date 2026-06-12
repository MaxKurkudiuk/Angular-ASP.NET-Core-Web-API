import { Routes } from '@angular/router';
import { UserComponent } from './shared/components/user/user.component';
import { RegistrationComponent } from './shared/components/user/registration/registration.component';
import { LoginComponent } from './shared/components/user/login/login.component';
import { DashboardComponent } from './shared/components/dashboard/dashboard-component';
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
