import { Component, inject, OnInit, signal } from '@angular/core';
import { UserService } from '../../../core/services/user.service';
import { HideIfClaimsNotMetDirective } from '../../directives/hide-if-claims-not-met.directive';
import { claimReq } from '../../utils/claimReq-utils';

@Component({
  selector: 'app-dashboard',
  imports: [HideIfClaimsNotMetDirective],
  templateUrl: './dashboard.component.html',
  styles: ``,
})
export class DashboardComponent implements OnInit {
  private userService = inject(UserService);
  claimReq = claimReq;
  fullName = signal<string>('');

  ngOnInit(): void {
    this.userService.getUserProfile().subscribe({
      next: (res: any) => {
        this.fullName.set(res.fullName);
      },
      error: (err: any) => console.log('error while retreaving user profile: \n', err)
    });
  }
}
