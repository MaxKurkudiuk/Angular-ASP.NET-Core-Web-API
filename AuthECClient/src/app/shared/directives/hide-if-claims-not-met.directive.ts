import { Directive, effect, ElementRef, inject, Input } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';

@Directive({
  selector: '[appHideIfClaimsNotMet]',
})
export class HideIfClaimsNotMetDirective {
  @Input("appHideIfClaimsNotMet") claimReq!: Function;

  private authService = inject(AuthService);
  private elementRef = inject(ElementRef);

  constructor() {
    effect(() => {
      const claims = this.authService.claims();
      if (this.claimReq && !this.claimReq(claims))
        this.elementRef.nativeElement.style.display = "none";
      else
        this.elementRef.nativeElement.style.display = "";
    });
  }
}
