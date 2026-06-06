import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { FirstKeyPipe } from '../../shared/pipes/first-key.pipe';
import { AuchService } from '../../shared/services/auch.service';

@Component({
    selector: 'app-registration',
    imports: [ReactiveFormsModule, FirstKeyPipe],
    templateUrl: './registration.component.html',
    styles: ``,
})
export class Registration {
    public formBuilder = inject(FormBuilder);
    private service = inject(AuchService);
    isSubmitted: boolean = false;

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
            Validators.pattern(/(?=.*[^a-zA-Z0-9 ])/)]],
        confirmPassword: ['']
    }, { validators: this.passwordMatchValidator });

    onSubmit() {
        this.isSubmitted = true;
        if (this.form.valid)
            this.service.createUser(this.form.value)
            .subscribe({
                next: res => {
                    console.log(res);
                },
                error: err => console.log('error', err)
            });
    }

    hasDisplayableError(controlName: string): Boolean {
        const control = this.form.get(controlName);
        return Boolean(control?.invalid) &&
            (this.isSubmitted || Boolean(control?.touched))
    }
}
