import {
  Component,
  OnInit
}
from '@angular/core';

import {
  FormBuilder,
  FormGroup,
  Validators
}
from '@angular/forms';

import {
  ActivatedRoute,
  Router
}
from '@angular/router';

import { Employee }
from '../../services/employee';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-employee-edit',

  standalone: false,

  templateUrl:
    './employee-edit.html',

  styleUrl:
    './employee-edit.css'
})

export class EmployeeEdit
implements OnInit {

  employeeForm: FormGroup;

  employeeId = 0;

  loading = false;

  role = '';

  constructor(
    private fb: FormBuilder,

    private employeeService:
      Employee,

    private authService: Auth,

    private route:
      ActivatedRoute,

    private router: Router
  )
  {
    this.employeeForm =
      this.fb.group({

        employeeId: [''],

        firstName: [

          '',

          [
            Validators.required,
            Validators.minLength(2)
          ]
        ],

        lastName: [

          '',

          [
            Validators.required,
            Validators.minLength(2)
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

        departmentId: [

          '',

          Validators.required
        ],

        roleId: [

          '',

          Validators.required
        ],
      });
  }

  get f() {

    return this.employeeForm.controls;
  }

  ngOnInit(): void {

    this.role =
      this.authService.getRole();

    this.employeeId = Number(
      this.route.snapshot
        .paramMap.get('id')
    );

    this.loadEmployee();
  }

  loadEmployee(): void {

    this.loading = true;

    this.employeeService
      .getById(this.employeeId)

      .subscribe({

        next: (response) =>
        {
          this.loading = false;

          this.employeeForm
            .patchValue(
              {
                employeeId: response.data.employeeId,

                firstName: response.data.firstName,

                lastName: response.data.lastName,

                phoneNumber: response.data.phoneNumber,

                gender: response.data.genderId,

                departmentId: response.data.departmentId,

                roleId: response.data.roleId
              }
            );

          if (this.role === 'Employee')
          {
            this.employeeForm
              .get('departmentId')
              ?.disable();

            this.employeeForm
              .get('roleId')
              ?.disable();
          }
        },

        error: () =>
        {
          this.loading = false;
        }
      });
  }

  update(): void {

    if (
      this.employeeForm.invalid
    )
    {
      this.employeeForm
        .markAllAsTouched();

      return;
    }

    this.loading = true;

    this.employeeService
      .update(
        this.employeeForm.getRawValue()
      )

      .subscribe({

        next: () =>
        {
          this.loading = false;

          alert(
            'Employee Updated Successfully'
          );

          this.router.navigate([
            '/employees'
          ]);
        },

        error: () =>
        {
          this.loading = false;
        }
      });
  }
}