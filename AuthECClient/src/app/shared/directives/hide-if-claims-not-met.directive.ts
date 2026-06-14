import { Directive, ElementRef, inject, Input, OnInit } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';

@Directive({
  selector: '[appHideIfClaimsNotMet]',
})
export class HideIfClaimsNotMetDirective implements OnInit {
  @Input("appHideIfClaimsNotMet") claimReq!: Function;

  private authService = inject(AuthService);
  private elementRef = inject(ElementRef);

  ngOnInit(): void {
    const claims = this.authService.getClaims();
    // console.log("claims:", claims);
    if (!this.claimReq(claims))
      this.elementRef.nativeElement.style.display = "none";
  }
}
