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
  catchError,
  forkJoin,
  of
}
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

  currentEmployeeDepartmentId = 0;

  currentEmployeeAttendanceRecords: any[] = [];

  attendanceRecords: any[] = [];

  searchForm: FormGroup;

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
        ],
        attendanceDate: [
          ''
        ]
      });

    this.searchForm = this.fb.group({
      employeeName: [''],
      minHours: ['']
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

    return [...this.currentEmployeeAttendanceRecords]
      .filter(record =>
        Number(record.empId) === Number(this.currentEmployeeId))
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

          this.currentEmployeeDepartmentId =
            exact?.departmentId ?? 0;

          if (this.currentEmployeeId > 0)
          {
            this.attendanceForm
              .patchValue({
                empId: this.currentEmployeeId
              });

            this.attendanceForm
              .get('empId')
              ?.disable();

            this.refreshAttendanceData();
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

          this.refreshAttendanceData();
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

          this.currentEmployeeAttendanceRecords =
            [...(response.data ?? [])].sort((left: any, right: any) =>
              new Date(right.checkIn ?? 0).getTime() - new Date(left.checkIn ?? 0).getTime()
            );

          if (this.role === 'Employee')
          {
            this.attendanceRecords =
              [...this.currentEmployeeAttendanceRecords];
          }
        },
        error: () =>
        {
          this.loading = false;
        }
      });
  }

  private refreshAttendanceData(): void {

    if (this.role === 'Employee')
    {
      this.loadOwnAttendance();
      return;
    }

    if (this.role === 'Manager' && !this.currentEmployeeDepartmentId)
    {
      return;
    }

    if (this.visibleEmployees.length === 0)
    {
      return;
    }

    this.loadAttendanceForVisibleEmployees();
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

    const employees =
      this.role === 'Manager'
        ? this.visibleEmployees.filter(employee =>
            Number(employee.departmentId) ===
            Number(this.currentEmployeeDepartmentId))
        : this.visibleEmployees;

    const requests =
      employees.map(
        employee =>
          this.attendanceService
            .getMonthlyAttendance(
              employee.employeeId,
              month,
              year
            )
            .pipe(
              catchError(() => of({ data: [] }))
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
              response => response.data ?? [])
              .sort((left: any, right: any) =>
                new Date(right.checkIn ?? 0).getTime() - new Date(left.checkIn ?? 0).getTime()
              );
        },
        error: () =>
        {
          this.loading = false;
        }
      });
  }

  get filteredAttendanceRecords(): any[] {
    const employeeTerm = String(this.searchForm.value.employeeName ?? '').trim().toLowerCase();
    const attendanceDate = this.reportForm.value.attendanceDate;
    const minHours = Number(this.searchForm.value.minHours);

    return this.attendanceRecords.filter(record => {
      const matchesEmployee = !employeeTerm || String(record.employeeName ?? '').toLowerCase().includes(employeeTerm);
      const matchesDate = !attendanceDate || this.formatDate(record.checkIn) === attendanceDate;
      const matchesHours = !minHours || minHours <= 0 || (record.totalHours != null && record.totalHours < minHours);

      return matchesEmployee && matchesDate && matchesHours;
    });
  }

  search(): void {
    this.attendanceRecords = [...this.attendanceRecords];
  }

  formatHours(totalHours: number): string {
    if (totalHours == null || totalHours < 0) {
      return '-';
    }

    const totalSeconds = Math.round(totalHours * 3600);
    const hours = Math.floor(totalSeconds / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    const seconds = totalSeconds % 60;

    return `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
  }

  private formatDate(value: any): string {
    if (!value) {
      return '';
    }

    const date = new Date(value);
    const year = date.getFullYear();
    const month = `${date.getMonth() + 1}`.padStart(2, '0');
    const day = `${date.getDate()}`.padStart(2, '0');

    return `${year}-${month}-${day}`;
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
        next: (response) =>
        {
          this.loading = false;

          const createdAttendance = response?.data ?? response;

          if (createdAttendance)
          {
            this.currentEmployeeAttendanceRecords = [
              createdAttendance,
              ...this.currentEmployeeAttendanceRecords.filter(record =>
                Number(record.attendanceId) !== Number(createdAttendance.attendanceId))
            ].sort((left: any, right: any) =>
              new Date(right.checkIn ?? 0).getTime() - new Date(left.checkIn ?? 0).getTime()
            );

            if (this.role === 'Employee')
            {
              this.attendanceRecords =
                [...this.currentEmployeeAttendanceRecords];
            }
          }

          this.feedback.success(
            'Checked in',
            'Attendance has been recorded successfully.'
          );

          this.attendanceForm.reset();

          this.attendanceForm
            .patchValue({
              empId: this.currentEmployeeId
            });

          this.refreshAttendanceData();
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
        next: (response) =>
        {
          this.loading = false;

          const updatedAttendance = response?.data ?? response;

          if (updatedAttendance)
          {
            this.currentEmployeeAttendanceRecords =
              this.currentEmployeeAttendanceRecords.map(record =>
                Number(record.attendanceId) === Number(updatedAttendance.attendanceId)
                  ? updatedAttendance
                  : record
              );

            if (this.role === 'Employee')
            {
              this.attendanceRecords =
                [...this.currentEmployeeAttendanceRecords];
            }
          }

          this.feedback.info(
            'Checked out',
            'Attendance has been closed for the day.'
          );

          this.refreshAttendanceData();
        },
        error: () =>
        {
          this.loading = false;
        }
      });
  }
}