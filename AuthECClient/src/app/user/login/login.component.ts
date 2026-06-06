import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styles: ``,
})
export class LoginComponent {
    formBuilder = inject(FormBuilder);

    form = this.formBuilder.group({
        email: ['', Validators.required],
        password: ['', Validators.required]
    });
}
