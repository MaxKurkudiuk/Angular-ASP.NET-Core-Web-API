import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterLinkWithHref, RouterLink } from '@angular/router';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { HideIfClaimsNotMetDirective } from '../../shared/directives/hide-if-claims-not-met.directive';
import { claimReq } from '../../shared/utils/claimReq-utils';

@Component({
  selector: 'app-main-layaut',
  imports: [RouterOutlet, RouterLinkWithHref, RouterLink, HideIfClaimsNotMetDirective],
  templateUrl: './main-layout.html',
  styles: ``,
})
export class MainLayoutComponent {
  claimReq = claimReq;
  private router = inject(Router);
  private authService = inject(AuthService);

    onLogout() {
    this.authService.deleteToken();
    this.router.navigateByUrl('/signin');
  }
}
