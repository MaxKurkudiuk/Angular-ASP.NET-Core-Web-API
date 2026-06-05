import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-registration',
  imports: [ReactiveFormsModule],
  templateUrl: './registration.component.html',
  styles: ``,
})
export class Registration {
    formBuilder = inject(FormBuilder);

    form = this.formBuilder.group({
        fullName : [''],
        email : [''],
        password : [''],
        confirmPassword : ['']
    });

    onSubmit(){
        console.log(this.form.value);
    }
}
