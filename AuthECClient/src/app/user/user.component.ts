import { Component, inject } from '@angular/core';
import { ChildrenOutletContexts, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-user',
  imports: [RouterOutlet],
  templateUrl: './user.component.html',
  styles: ``
})
export class UserComponent {
    private context = inject(ChildrenOutletContexts);

    getRouteUrl(){
        return this.context.getContext('primary')?.route?.url;
    }
}
