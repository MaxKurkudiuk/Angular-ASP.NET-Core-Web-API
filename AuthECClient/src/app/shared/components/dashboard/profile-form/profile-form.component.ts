import { Component, EventEmitter, inject, input, output, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../../../../core/services/user.service';
import { AppUser } from '../../../../core/models/user/app-user';

@Component({
  selector: 'app-profile-form',
  imports: [ReactiveFormsModule],
  templateUrl: './profile-form.component.html',
  styleUrl: './profile-form.component.css',
})
export class ProfileFormComponent implements OnInit {
  private formBuilder = inject(FormBuilder);
  private userService = inject(UserService);
  private toastr = inject(ToastrService);

  user = input.required<AppUser>();
  close = output<void>();

  form = this.formBuilder.group({
    fullName: ['', Validators.required],
    age: [''],
    gender: [''],
    libraryID: [''],
  });

  isSubmitted: boolean = false;

  ngOnInit(): void {
    const u = this.user();
    this.form.patchValue({
      fullName: u.fullName || '',
      age: u.age || '',
      gender: u.gender || '',
      libraryID: u.libraryID || '',
    });
  }

  onSubmit() {
    this.isSubmitted = true;
    if (this.form.valid) {
      const value = this.form.value;
      const profileData: { fullName?: string; age?: number; gender?: string; libraryID?: string } = {};
      if (value.fullName != null && value.fullName !== '') profileData.fullName = value.fullName;
      if (value.age != null && value.age !== '') profileData.age = Number.parseInt(value.age);
      if (value.gender != null && value.gender !== '') profileData.gender = value.gender;
      if (value.libraryID != null && value.libraryID !== '') profileData.libraryID = value.libraryID;
      this.userService.updateProfile(profileData).subscribe({
        next: () => {
          this.toastr.success('Profile updated successfully');
          this.close.emit();
        },
        error: (err: any) => {
          this.toastr.error('Failed to update profile');
          console.error(err);
        },
      });
    }
  }

  hasDisplayableError(controlName: string): boolean {
    const control = this.form.get(controlName);
    return Boolean(control?.invalid) &&
      (this.isSubmitted || Boolean(control?.touched) || Boolean(control?.dirty));
  }

  onBlurred() {
    this.isSubmitted = true;
  }
}
