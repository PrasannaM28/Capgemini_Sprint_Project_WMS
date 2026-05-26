import { Component }
from '@angular/core';

import { OnInit }
from '@angular/core';

import {
  FormBuilder,
  FormGroup,
  Validators
}
from '@angular/forms';

import { HttpClient }
from '@angular/common/http';

import { Router }
from '@angular/router';

import { Department }
from '../../services/department';
import { Employee }
from '../../services/employee';
import { UiFeedbackService }
from '../../shared/ui-feedback/ui-feedback.service';
import { environment }
from '../../../environments/environment';

@Component({
  selector: 'app-employee-create',

  standalone: false,

  templateUrl:
    './employee-create.html',

  styleUrl:
    './employee-create.css'
})

export class EmployeeCreate implements OnInit {

  employeeForm: FormGroup;

  maxDobDate: string;

  maxDojDate: string;

  departments: any[] = [];

  roles: any[] = [];

  constructor(
    private fb: FormBuilder,

    private employeeService:
      Employee,

    private departmentService: Department,

    private feedback: UiFeedbackService,

    private http: HttpClient,

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

  ngOnInit(): void {

    this.loadDepartments();
    this.loadRoles();
  }

  loadDepartments(): void {

    this.departmentService
      .getAll()
      .subscribe({
        next: (response) =>
        {
          this.departments = response.data ?? [];
        }
      });
  }

  loadRoles(): void {

    this.http
      .get(`${environment.apiUrl}/role`)
      .subscribe({
        next: (response) =>
        {
          this.roles = (response as any).data ?? [];
        }
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
          this.feedback.success(
            'Employee created',
            'The new employee was added successfully.'
          );

          this.router.navigate([
            '/employees'
          ]);
        },

        error: () =>
        {
          this.feedback.error(
            'Unable to create employee',
            'Please review the form and try again.'
          );
        }
      });
  }
}
