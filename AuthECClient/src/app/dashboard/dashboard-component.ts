import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  imports: [],
  templateUrl: './dashboard-component.html',
  styles: ``,
})
export class DashboardComponent {
    private router = inject(Router);

    onLogout(){
        localStorage.removeItem('token');
        this.router.navigateByUrl('/signin');
    }
}
