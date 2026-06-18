import { Component, inject, OnInit, signal } from '@angular/core';
import { UserService } from '../../../core/services/user.service';
import { HideIfClaimsNotMetDirective } from '../../directives/hide-if-claims-not-met.directive';
import { claimReq } from '../../utils/claimReq-utils';
import { AppUser } from '../../../core/models/user/app-user';

@Component({
  selector: 'app-dashboard',
  imports: [HideIfClaimsNotMetDirective],
  templateUrl: './dashboard.component.html',
  styles: ``,
})
export class DashboardComponent implements OnInit {
  private userService = inject(UserService);
  claimReq = claimReq;
  appUser = signal<AppUser>(new AppUser());

  ngOnInit(): void {
    this.userService.getUserProfile().subscribe({
      next: (res: any) => {
        // const user = new AppUser();    // variant 1
        // user.fullName = res.fullName;
        // user.gender = res.gender;
        // this.appUser.set(user);

        // const user = new AppUser();    // variant 2
        // Object.assign(user, res);
        // this.appUser.set(user);
        
        this.appUser.set(Object.assign(new AppUser(), res));  // variant 3
      },
      error: (err: any) => console.log('error while retreaving user profile: \n', err)
    });
  }
}
