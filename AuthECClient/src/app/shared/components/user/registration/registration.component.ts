import { Component, inject } from '@angular/core';
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
export class RegistrationComponent {
  formBuilder = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  toastr = inject(ToastrService)
  isSubmitted: boolean = false;
  isSubmitting: boolean = false;
  currentStep: number = 1;

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
    confirmPassword: [''],
    gender: ['', Validators.required],
    age: ['', [Validators.required, Validators.min(1), Validators.max(120)]],
    libraryID: [null]
  }, { validators: this.passwordMatchValidator });

  nextStep() {
    const step1Controls = ['fullName', 'email', 'password', 'confirmPassword'];
    step1Controls.forEach(name => this.form.get(name)?.markAsTouched());
    if (this.form.controls.fullName.valid && this.form.controls.email.valid &&
        this.form.controls.password.valid && !this.form.controls.confirmPassword.errors) {
      this.currentStep = 2;
    }
  }

  prevStep() {
    this.currentStep = 1;
  }

  onSubmit() {
    this.isSubmitted = true;
    if (this.form.invalid) return;

    const formValue: any = { ...this.form.value };
    const email = formValue.email;
    const password = formValue.password;
    delete formValue.confirmPassword;
    formValue.age = Number(formValue.age);
    formValue.libraryID = formValue.libraryID ? Number(formValue.libraryID) : null;
    this.isSubmitting = true;
    this.authService.createUser(formValue)
      .subscribe({
        next: (res: any) => {
          if (res.succeeded) {
            this.authService.signin({ email, password }).subscribe({
              next: (loginRes: any) => {
                this.authService.saveToken(loginRes.token);
                this.form.reset();
                this.isSubmitted = false;
                this.isSubmitting = false;
                this.currentStep = 1;
                this.toastr.success('New user created!', 'Registration succesful');
                this.router.navigate(['/dashboard']);
              },
              error: () => {
                this.isSubmitting = false;
                this.router.navigate(['/signin']);
              }
            });
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
          this.isSubmitting = false;
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
