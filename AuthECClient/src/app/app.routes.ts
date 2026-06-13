import { Routes } from '@angular/router';
import { UserComponent } from './shared/components/user/user.component';
import { RegistrationComponent } from './shared/components/user/registration/registration.component';
import { LoginComponent } from './shared/components/user/login/login.component';
import { DashboardComponent } from './shared/components/dashboard/dashboard.component';
import { authGuard, isNotLoggedInGuard } from './core/guards/auth-guard';
import { AdminOnlyComponent } from './shared/components/authorizeDemo/admin-only/admin-only.component';
import { AdminOrTeacherComponent } from './shared/components/authorizeDemo/admin-or-teacher/admin-or-teacher.component';
import { ApplyForMaternityLeaveComponent } from './shared/components/authorizeDemo/apply-for-maternity-leave/apply-for-maternity-leave.component';
import { LibraryMembersOnlyComponent } from './shared/components/authorizeDemo/library-members-only/library-members-only.component';
import { Under10AndFemaleComponent } from './shared/components/authorizeDemo/under10-and-female/under10-and-female.component';
import { MainLaoautComponent } from './layouts/main-layout/main-layout';

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
      path:'', component: MainLaoautComponent,
      children: [
        { 
            path: 'dashboard', component: DashboardComponent,
            canActivate: [authGuard]
        },
        { 
            path: 'admin-only', component: AdminOnlyComponent,
            canActivate: [authGuard]
        },
        { 
            path: 'admin-or-teacher', component: AdminOrTeacherComponent,
            canActivate: [authGuard]
        },
        { 
            path: 'apply-for-maternity-leave', component: ApplyForMaternityLeaveComponent,
            canActivate: [authGuard]
        },
        { 
            path: 'library-members-only', component: LibraryMembersOnlyComponent,
            canActivate: [authGuard]
        },
        { 
            path: 'under10-and-female', component: Under10AndFemaleComponent,
            canActivate: [authGuard]
        }
      ]
    },
];
