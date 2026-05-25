import { Component }
from '@angular/core';

import {
  FormBuilder,
  FormGroup,
  Validators
}
from '@angular/forms';

import { Router }
from '@angular/router';

import { Auth }
from '../../services/auth';

@Component({
  selector: 'app-login',

  standalone: false,

  templateUrl: './login.html',

  styleUrl: './login.css',
})

export class Login {

  loginForm: FormGroup;

  errorMessage = '';

  loading = false;

  constructor(
    private fb: FormBuilder,

    private authService:
      Auth,

    private router: Router
  )
  {
    this.loginForm =
      this.fb.group({

        username: [

          '',

          [
            Validators.required,
            Validators.minLength(3)
          ]
        ],

        password: [

          '',

          [
            Validators.required,
            Validators.minLength(6)
          ]
        ]
      });
  }

  get f() {

    return this.loginForm.controls;
  }

  login(): void {

    if (
      this.loginForm.invalid
    )
    {
      this.loginForm
        .markAllAsTouched();

      return;
    }

    this.loading = true;

    this.authService
      .login(this.loginForm.value)

      .subscribe({

        next: () =>
        {
          this.loading = false;

          this.router.navigate([
            '/dashboard'
          ]);
        },

        error: () =>
        {
          this.loading = false;

          this.errorMessage =
            'Invalid credentials';
        }
      });
  }
}
