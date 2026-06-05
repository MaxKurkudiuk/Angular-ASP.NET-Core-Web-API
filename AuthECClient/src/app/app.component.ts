import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { User } from "./user/user.component";

@Component({
  selector: 'app-root',
  imports: [/*RouterOutlet,*/ User],
  templateUrl: './app.component.html',
  styles: [],
})
export class App {
  protected readonly title = signal('AuthECClient');
}
