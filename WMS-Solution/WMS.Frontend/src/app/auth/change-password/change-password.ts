import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

import {
  FormBuilder,
  FormGroup,
  Validators
} from '@angular/forms';

import { Router } from '@angular/router';

import { Auth } from '../../services/auth';

@Component({
  selector: 'app-change-password',

  standalone: true,

  imports: [CommonModule, ReactiveFormsModule],

  template: `
    <div class="change-password-page">

      <div class="container py-5">

        <div class="row justify-content-center">

          <div class="col-md-5 col-lg-4">

            <div class="card shadow-sm change-password-card">

              <div class="card-header text-center border-0 bg-transparent pt-4">

                <h3 class="mb-0">
                  Change Password
                </h3>

              </div>

              <div class="card-body p-4">

                <div
                  *ngIf="errorMessage"
                  class="alert alert-danger">

                  {{ errorMessage }}

                </div>

                <form
                  [formGroup]="changePasswordForm"
                  (ngSubmit)="submit()">

                  <div class="mb-3">

                    <label class="form-label">
                      Current Password
                    </label>

                    <input
                      type="password"
                      class="form-control"
                      formControlName="currentPassword">

                    <div
                      class="text-danger mt-1"
                      *ngIf="f['currentPassword'].touched && f['currentPassword'].invalid">

                      Current password is required

                    </div>

                  </div>

                  <div class="mb-3">

                    <label class="form-label">
                      New Password
                    </label>

                    <input
                      type="password"
                      class="form-control"
                      formControlName="newPassword">

                    <div
                      class="text-danger mt-1"
                      *ngIf="f['newPassword'].touched && f['newPassword'].invalid">

                      <div *ngIf="f['newPassword'].errors?.['required']">
                        New password is required
                      </div>

                      <div *ngIf="f['newPassword'].errors?.['minlength']">
                        Minimum 6 characters required
                      </div>

                    </div>

                  </div>

                  <div class="mb-4">

                    <label class="form-label">
                      Confirm New Password
                    </label>

                    <input
                      type="password"
                      class="form-control"
                      formControlName="confirmNewPassword">

                    <div
                      class="text-danger mt-1"
                      *ngIf="f['confirmNewPassword'].touched && f['confirmNewPassword'].invalid">

                      Confirm new password is required

                    </div>

                  </div>

                  <button
                    type="submit"
                    class="btn btn-primary w-100"
                    [disabled]="loading">

                    {{ loading ? 'Updating...' : 'Update Password' }}

                  </button>

                </form>

              </div>

            </div>

          </div>

        </div>

      </div>

    </div>
  `,

  styles: [`
    .change-password-page {
      min-height: calc(100vh - 72px);
      background: linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #334155 100%);
    }

    .change-password-card {
      border: none;
      border-radius: 18px;
    }

    .change-password-card .card-header h3 {
      color: #0f172a;
      font-weight: 700;
    }
  `]
})

export class ChangePassword {

  changePasswordForm: FormGroup;

  loading = false;

  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: Auth,
    private router: Router
  )
  {
    this.changePasswordForm = this.fb.group({
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmNewPassword: ['', [Validators.required]]
    });
  }

  get f() {
    return this.changePasswordForm.controls;
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
      return;
    }

    this.loading = true;

    this.authService.changePassword(value).subscribe({
      next: () => {
        this.loading = false;
        alert('Password changed successfully.');
        this.router.navigate(['/dashboard']);
      },
      error: (error: any) => {
        this.loading = false;
        this.errorMessage =
          error?.error?.message || 'Unable to change password.';
      }
    });
  }
}