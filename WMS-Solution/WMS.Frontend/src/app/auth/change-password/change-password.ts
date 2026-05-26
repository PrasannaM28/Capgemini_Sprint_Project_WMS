import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { Auth } from '../../services/auth';
import { UiFeedbackService } from '../../shared/ui-feedback/ui-feedback.service';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './change-password.html',
  styleUrl: './change-password.css'
})
export class ChangePassword {
  changePasswordForm: FormGroup;
  loading = false;
  errorMessage = '';
  showCurrentPassword = false;
  showNewPassword = false;
  showConfirmNewPassword = false;

  constructor(
    private fb: FormBuilder,
    private authService: Auth,
    private router: Router,
    private feedback: UiFeedbackService
  ) {
    this.changePasswordForm = this.fb.group({
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmNewPassword: ['', [Validators.required]]
    });
  }

  get f() {
    return this.changePasswordForm.controls;
  }

  toggleCurrentPassword(): void {
    this.showCurrentPassword = !this.showCurrentPassword;
  }

  toggleNewPassword(): void {
    this.showNewPassword = !this.showNewPassword;
  }

  toggleConfirmNewPassword(): void {
    this.showConfirmNewPassword = !this.showConfirmNewPassword;
  }

  submit(): void {
    this.errorMessage = '';

    if (this.changePasswordForm.invalid) {
      this.changePasswordForm.markAllAsTouched();
      return;
    }

    const value = this.changePasswordForm.value;

    if (value.newPassword !== value.confirmNewPassword) {
      this.errorMessage = 'New password and confirmation must match.';
      this.feedback.warning('Password mismatch', this.errorMessage);
      return;
    }

    this.loading = true;

    this.authService.changePassword(value).subscribe({
      next: () => {
        this.loading = false;
        this.feedback.success('Password updated', 'Your credentials were changed successfully.');
        this.router.navigate(['/dashboard']);
      },
      error: (error: any) => {
        this.loading = false;
        this.errorMessage = error?.error?.message || 'Unable to change password.';
      }
    });
  }
}