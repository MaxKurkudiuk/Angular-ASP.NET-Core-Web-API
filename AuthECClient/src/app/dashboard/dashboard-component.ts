import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { TOKEN_KEY } from '../shared/constants';

@Component({
  selector: 'app-dashboard',
  imports: [],
  templateUrl: './dashboard-component.html',
  styles: ``,
})
export class DashboardComponent {
    private router = inject(Router);

    onLogout(){
        localStorage.removeItem(TOKEN_KEY);
        this.router.navigateByUrl('/signin');
    }
}
