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

import { HttpClient }
from '@angular/common/http';

import {
  ActivatedRoute,
  Router
}
from '@angular/router';

import { Department }
from '../../services/department';
import { Employee }
from '../../services/employee';
import { Auth } from '../../services/auth';
import { UiFeedbackService }
from '../../shared/ui-feedback/ui-feedback.service';
import { environment }
from '../../../environments/environment';

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

  departments: any[] = [];

  roles: any[] = [];

  get pageHeading(): string {
    return this.role === 'Admin' ? 'Edit Employee' : 'My Profile';
  }

  constructor(
    private fb: FormBuilder,

    private employeeService:
      Employee,

    private departmentService: Department,

    private http: HttpClient,

    private authService: Auth,

    private feedback: UiFeedbackService,

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

    this.loadDepartments();
    this.loadRoles();

    this.employeeId = Number(
      this.route.snapshot
        .paramMap.get('id')
    );

    this.loadEmployee();
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

          if (this.role !== 'Admin')
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

          this.feedback.success(
            this.role === 'Admin' ? 'Employee updated' : 'Profile updated',
            this.role === 'Admin'
              ? 'The employee record was saved successfully.'
              : 'Your profile was saved successfully.'
          );

          this.router.navigate([
            this.role === 'Admin' ? '/employees' : '/dashboard'
          ]);
        },

        error: () =>
        {
          this.loading = false;
        }
      });
  }
}