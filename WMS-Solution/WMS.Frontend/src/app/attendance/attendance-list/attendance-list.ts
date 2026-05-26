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

import { forkJoin }
from 'rxjs';

import { Attendance }
from '../../services/attendance';
import { Employee }
from '../../services/employee';
import { Auth }
from '../../services/auth';
import { UiFeedbackService }
from '../../shared/ui-feedback/ui-feedback.service';

@Component({
  selector: 'app-attendance-list',

  standalone: false,

  templateUrl:
    './attendance-list.html',

  styleUrl:
    './attendance-list.css'
})

export class AttendanceList
implements OnInit {

  attendanceForm: FormGroup;

  reportForm: FormGroup;

  loading = false;

  role = '';

  username = '';

  currentEmployeeId = 0;

  attendanceRecords: any[] = [];

  visibleEmployees: any[] = [];

  constructor(
    private fb: FormBuilder,

    private attendanceService:
      Attendance,

    private employeeService:
      Employee,

    private authService:
      Auth,

    private feedback: UiFeedbackService
  )
  {
    const now = new Date();

    this.attendanceForm =
      this.fb.group({
        empId: [
          '',
          Validators.required
        ],
        workMode: [
          '',
          Validators.required
        ]
      });

    this.reportForm =
      this.fb.group({
        month: [
          now.getMonth() + 1,
          [Validators.required]
        ],
        year: [
          now.getFullYear(),
          [Validators.required]
        ]
      });
  }

  get f() {

    return this.attendanceForm.controls;
  }

  ngOnInit(): void {

    this.role =
      this.authService.getRole();

    this.username =
      this.authService.getUsername();

    if (this.username)
    {
      this.resolveCurrentEmployee();
    }

    if (this.role !== 'Employee')
    {
      this.loadVisibleEmployees();
    }
  }

  get showEmployeeActions(): boolean {

    return this.currentEmployeeId > 0;
  }

  get openAttendance(): any {

    return [...this.attendanceRecords]
      .filter(record =>
        record.empId === this.currentEmployeeId)
      .sort((a, b) =>
        new Date(b.checkIn).getTime() -
        new Date(a.checkIn).getTime())
      .find(record => !record.checkOut);
  }

  private resolveCurrentEmployee(): void {

    if (!this.username)
    {
      return;
    }

    this.employeeService
      .search(this.username)
      .subscribe({
        next: (response) =>
        {
          const exact =
            (response.data ?? [])
              .find((employee: any) =>
                String(employee.email ?? '')
                  .toLowerCase() ===
                this.username.toLowerCase());

          this.currentEmployeeId =
            exact?.employeeId ?? 0;

          if (this.currentEmployeeId > 0)
          {
            this.attendanceForm
              .patchValue({
                empId: this.currentEmployeeId
              });

            this.attendanceForm
              .get('empId')
              ?.disable();

              if (this.role === 'Employee')
              {
                this.loadOwnAttendance();
              }
          }
        }
      });
  }

  private loadVisibleEmployees(): void {

    this.loading = true;

    this.employeeService
      .getAll()
      .subscribe({
        next: (response) =>
        {
          this.visibleEmployees =
            response.data ?? [];

          this.loadAttendanceForVisibleEmployees();
        },
        error: () =>
        {
          this.loading = false;
        }
      });
  }

  private loadOwnAttendance(): void {

    if (!this.currentEmployeeId)
    {
      return;
    }

    const month =
      this.reportForm.value.month;

    const year =
      this.reportForm.value.year;

    this.loading = true;

    this.attendanceService
      .getMonthlyAttendance(
        this.currentEmployeeId,
        month,
        year
      )
      .subscribe({
        next: (response) =>
        {
          this.loading = false;

          this.attendanceRecords =
            response.data ?? [];
        },
        error: () =>
        {
          this.loading = false;
        }
      });
  }

  loadAttendanceForVisibleEmployees(): void {

    if (this.reportForm.invalid)
    {
      return;
    }

    if (this.role === 'Employee')
    {
      this.loadOwnAttendance();
      return;
    }

    const month =
      this.reportForm.value.month;

    const year =
      this.reportForm.value.year;

    const requests =
      this.visibleEmployees.map(
        employee =>
          this.attendanceService
            .getMonthlyAttendance(
              employee.employeeId,
              month,
              year
            )
      );

    if (requests.length === 0)
    {
      this.attendanceRecords = [];
      this.loading = false;
      return;
    }

    this.loading = true;

    forkJoin(requests)
      .subscribe({
        next: (responses) =>
        {
          this.loading = false;

          this.attendanceRecords =
            responses.flatMap(
              response => response.data ?? []);
        },
        error: () =>
        {
          this.loading = false;
        }
      });
  }

  checkIn(): void {

    if (
      this.attendanceForm.invalid ||
      !this.currentEmployeeId
    )
    {
      this.attendanceForm
        .markAllAsTouched();

      return;
    }

    this.loading = true;

    const payload = {
      empId: this.currentEmployeeId,
      workMode:
        this.attendanceForm.value.workMode,
      checkIn:
        new Date()
    };

    this.attendanceService
      .checkIn(payload)
      .subscribe({
        next: () =>
        {
          this.loading = false;

          this.feedback.success(
            'Checked in',
            'Attendance has been recorded successfully.'
          );

          this.attendanceForm.reset();

          this.attendanceForm
            .patchValue({
              empId: this.currentEmployeeId
            });

          if (this.role === 'Employee')
          {
            this.loadOwnAttendance();
          }
          else
          {
            this.loadAttendanceForVisibleEmployees();
          }
        },
        error: () =>
        {
          this.loading = false;
        }
      });
  }

  checkOut(): void {

    if (!this.openAttendance)
    {
      return;
    }

    this.loading = true;

    this.attendanceService
      .checkOut({
        attendanceId:
          this.openAttendance.attendanceId,
        checkOut: new Date()
      })
      .subscribe({
        next: () =>
        {
          this.loading = false;

          this.feedback.info(
            'Checked out',
            'Attendance has been closed for the day.'
          );

          if (this.role === 'Employee')
          {
            this.loadOwnAttendance();
          }
          else
          {
            this.loadAttendanceForVisibleEmployees();
          }
        },
        error: () =>
        {
          this.loading = false;
        }
      });
  }
}