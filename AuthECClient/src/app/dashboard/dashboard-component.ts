import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../shared/services/auth.service';

@Component({
  selector: 'app-dashboard',
  imports: [],
  templateUrl: './dashboard-component.html',
  styles: ``,
})
export class DashboardComponent {
    private router = inject(Router);
    private authService = inject(AuthService);

    onLogout(){
        this.authService.deleteToken();
        this.router.navigateByUrl('/signin');
    }
}
