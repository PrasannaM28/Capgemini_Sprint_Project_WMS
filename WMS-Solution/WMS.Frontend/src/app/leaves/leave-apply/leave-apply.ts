import { Component }
from '@angular/core';

import {
  FormBuilder,
  FormGroup,
  Validators
}
from '@angular/forms';

import { Auth } from '../../services/auth';
import { Employee } from '../../services/employee';
import { Leave }
from '../../services/leave';

@Component({
  selector: 'app-leave-apply',

  standalone: false,

  templateUrl:
    './leave-apply.html',

  styleUrl:
    './leave-apply.css'
})

export class LeaveApply {

  leaveForm: FormGroup;

  role = '';

  currentEmployeeId = 0;

  constructor(
    private fb: FormBuilder,

    private authService: Auth,

    private employeeService: Employee,

    private leaveService:
      Leave
  )
  {
    this.leaveForm =
      this.fb.group({

        empId: [

          '',

          Validators.required
        ],

        leaveType: [

          '',

          Validators.required
        ],

        reason: [

          '',

          [
            Validators.required,
            Validators.minLength(5)
          ]
        ],

        fromDate: [

          '',

          Validators.required
        ],

        toDate: [

          '',

          Validators.required
        ]
      });
  }

  ngOnInit(): void {

    this.role = this.authService.getRole();

    if (this.role === 'Employee') {
      this.resolveCurrentEmployee();
    }
  }

  get canApply(): boolean {

    return this.role === 'Employee';
  }

  get f() {

    return this.leaveForm.controls;
  }

  private resolveCurrentEmployee(): void {

    const username = this.authService.getUsername();

    if (!username) {
      return;
    }

    this.employeeService.search(username).subscribe({
      next: (response) => {
        const exact = (response.data ?? []).find((employee: any) =>
          String(employee.email ?? '').toLowerCase() === username.toLowerCase());

        this.currentEmployeeId = exact?.employeeId ?? 0;

        if (this.currentEmployeeId > 0) {
          this.leaveForm.patchValue({ empId: this.currentEmployeeId });
          this.leaveForm.get('empId')?.disable();
        }
      },
    });
  }

  apply(): void {

    if (
      this.leaveForm.invalid
    )
    {
      this.leaveForm
        .markAllAsTouched();

      return;
    }

    this.leaveService
      .apply(
        this.leaveForm.value
      )

      .subscribe({

        next: () =>
        {
          alert(
            'Leave Applied Successfully'
          );

          this.leaveForm.reset();
        }
      });
  }
}
