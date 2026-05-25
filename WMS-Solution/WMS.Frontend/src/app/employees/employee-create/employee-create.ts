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

import { Employee }
from '../../services/employee';

@Component({
  selector: 'app-employee-create',

  standalone: false,

  templateUrl:
    './employee-create.html',

  styleUrl:
    './employee-create.css'
})

export class EmployeeCreate {

  employeeForm: FormGroup;

  maxDobDate: string;

  maxDojDate: string;

  constructor(
    private fb: FormBuilder,

    private employeeService:
      Employee,

    private router: Router
  )
  {
    this.maxDojDate =
      new Date().toISOString().split('T')[0];

    this.maxDobDate =
      new Date(
        new Date().setFullYear(
          new Date().getFullYear() - 18
        )
      ).toISOString().split('T')[0];

    this.employeeForm =
      this.fb.group({

        firstName: [

          '',

          [
            Validators.required,
            Validators.minLength(2),
            Validators.maxLength(50)
          ]
        ],

        lastName: [

          '',

          [
            Validators.required,
            Validators.minLength(2),
            Validators.maxLength(50)
          ]
        ],

        email: [

          '',

          [
            Validators.required,
            Validators.email
          ]
        ],

        phoneNumber: [

          '',

          [
            Validators.required,

            Validators.pattern(
              '^[0-9]{10}$'
            )
          ]
        ],

        gender: [

          '',

          Validators.required
        ],

        dob: [

          '',

          Validators.required
        ],

        doj: [

          '',

          Validators.required
        ],

        departmentId: [

          '',

          Validators.required
        ],

        roleId: [

          '',

          Validators.required
        ]
      });
  }

  get f() {

    return this.employeeForm.controls;
  }

  create(): void {

    if (
      this.employeeForm.invalid
    )
    {
      this.employeeForm
        .markAllAsTouched();

      return;
    }

    this.employeeService
      .create(
        this.employeeForm.value
      )

      .subscribe({

        next: () =>
        {
          alert(
            'Employee Created Successfully'
          );

          this.router.navigate([
            '/employees'
          ]);
        },

        error: () =>
        {
          alert(
            'Unable to create employee'
          );
        }
      });
  }
}
