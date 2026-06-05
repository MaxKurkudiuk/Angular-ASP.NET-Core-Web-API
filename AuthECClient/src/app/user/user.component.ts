import { Component } from '@angular/core';
import { Registration } from "./registration/registration.component";

@Component({
  selector: 'app-user',
  imports: [Registration],
  templateUrl: './user.component.html',
  styles: ``,
})
export class User {}
