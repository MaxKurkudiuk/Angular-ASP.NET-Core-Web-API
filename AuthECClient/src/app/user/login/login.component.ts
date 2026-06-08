import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from "@angular/router";
import { AuthService } from '../../shared/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
    selector: 'app-login',
    imports: [ReactiveFormsModule, RouterLink],
    templateUrl: './login.component.html',
    styles: ``,
})
export class LoginComponent {
    formBuilder = inject(FormBuilder);
    private auchService = inject(AuthService);
    private router = inject(Router);
    private toastr = inject(ToastrService);
    isSubmitted: boolean = false;

    form = this.formBuilder.group({
        email: ['', Validators.required],
        password: ['', Validators.required]
    });

    onSubmit() {
        this.isSubmitted = true;
        if (this.form.valid) {
            this.auchService.signin(this.form.value).subscribe({
                next: (res: any) => {
                    this.auchService.saveToken(res.token);
                    this.router.navigateByUrl('/dashboard');
                },
                error:err => {
                    if (err.status == 400)
                        this.toastr.error('Incorrect email or password.', 'Login failed');
                    else
                        console.log('error during login:\n', err);
                }
            })
        }
    }

    hasDisplayableError(controlName: string): Boolean {
        const control = this.form.get(controlName);
        return Boolean(control?.invalid) &&
            (this.isSubmitted || Boolean(control?.touched) || Boolean(control?.dirty))
    }
}
