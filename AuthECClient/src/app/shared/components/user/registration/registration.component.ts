import { Component, inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { FirstKeyPipe } from '../../../pipes/first-key.pipe';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-registration',
  imports: [ReactiveFormsModule, FirstKeyPipe, RouterLink],
  templateUrl: './registration.component.html',
  styles: ``,
})
export class RegistrationComponent implements OnInit {
  formBuilder = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  toastr = inject(ToastrService)
  isSubmitted: boolean = false;

  ngOnInit(): void {
  }
  
  // custom validation
  passwordMatchValidator: ValidatorFn = (control: AbstractControl): null => {
    const password = control.get('password')
    const confirmPassword = control.get('confirmPassword')

    if (password && confirmPassword && password.value != confirmPassword.value)
      confirmPassword?.setErrors({ passwordMismatch: true })
    else
      confirmPassword?.setErrors(null)

    return null;
  }

  form = this.formBuilder.group({
    fullName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [
      Validators.required,
      Validators.minLength(6),
      uppercaseCharacterValidator(),
      lowercaseCharacterValidator(),
      Validators.pattern(/(?=.*[^a-zA-Z0-9 ])/)]],
    confirmPassword: ['']
  }, { validators: this.passwordMatchValidator });

  onSubmit() {
    this.isSubmitted = true;
    if (this.form.valid)
      this.authService.createUser(this.form.value)
        .subscribe({
          next: (res: any) => {
            if (res.succeeded) {
              this.form.reset();
              this.isSubmitted = false;
              this.toastr.success('New user created!', 'Registration succesful');
            }
          },
          error: err => {
            if (err.error.errors) {
              err.error.errors.forEach((x: any) => {
                switch (x.code) {
                  case "DuplicateUserName":
                    break;
                  case "DuplicateEmail":
                    this.toastr.error('Email already taken.', 'Registration failed');
                    break;
                  default:
                    this.toastr.error('Contact the developer', 'Registration failed');
                    console.log(x);
                }
              })
            } else
              console.log('error:', err);
          }
        });
  }

  hasDisplayableError(controlName: string): Boolean {
    const control = this.form.get(controlName);
    return Boolean(control?.invalid) &&
      (this.isSubmitted || Boolean(control?.touched) || Boolean(control?.dirty))
  }
}

export function uppercaseCharacterValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {

    const value = control.value;

    if (!value) {
      return null;
    }

    const hasUpperCase = /[A-Z]+/.test(value);

    return !hasUpperCase ? { uppercaseCharacter: true } : null;
  }
}

export function lowercaseCharacterValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {

    const value = control.value;

    if (!value) {
      return null;
    }

    const hasLowerCase = /[a-z]+/.test(value);

    return !hasLowerCase ? { lowercaseCharacter: true } : null;
  }
}