import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterLinkWithHref, RouterLink } from '@angular/router';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-main-layaut',
  imports: [RouterOutlet, RouterLinkWithHref, RouterLink],
  templateUrl: './main-layout.html',
  styles: ``,
})
export class MainLayoutComponent {
  private router = inject(Router);
  private authService = inject(AuthService);

    onLogout() {
    this.authService.deleteToken();
    this.router.navigateByUrl('/signin');
  }
}
