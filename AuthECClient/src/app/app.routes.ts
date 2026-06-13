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
import { MainLayoutComponent } from './layouts/main-layout/main-layout';

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
      path:'', component: MainLayoutComponent, canActivate: [authGuard],
      children: [
        { 
            path: 'dashboard', component: DashboardComponent
        },
        { 
            path: 'admin-only', component: AdminOnlyComponent,
            data: { claimReq: (c:any) => c.role == "Admin" }
        },
        { 
            path: 'admin-or-teacher', component: AdminOrTeacherComponent,
            data: { claimReq: (c:any) => c.role == "Admin" || c.role == "Teacher" }
        },
        { 
            path: 'apply-for-maternity-leave', component: ApplyForMaternityLeaveComponent
        },
        { 
            path: 'library-members-only', component: LibraryMembersOnlyComponent
        },
        { 
            path: 'under10-and-female', component: Under10AndFemaleComponent
        }
      ]
    },
];
