import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { Auth } from '../../services/auth';
import { Employee } from '../../services/employee';
import { Leave } from '../../services/leave';

@Component({
  selector: 'app-leave-list',
  standalone: false,
  templateUrl: './leave-list.html',
  styleUrl: './leave-list.css',
})
export class LeaveList {
  leaves: any[] = [];

  leaveForm: FormGroup;

  role = '';

  username = '';

  currentEmployeeId = 0;

  loading = false;

  submitting = false;

  constructor(
    private fb: FormBuilder,
    private leaveService: Leave,
    private authService: Auth,
    private employeeService: Employee
  ) {
    this.role = this.authService.getRole();

    this.leaveForm = this.fb.group({
      empId: ['', Validators.required],
      leaveType: ['', Validators.required],
      reason: ['', [Validators.required, Validators.minLength(5)]],
      fromDate: ['', Validators.required],
      toDate: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.role = this.authService.getRole();
    this.username = this.authService.getUsername();

    if (this.role === 'Employee') {
      this.resolveCurrentEmployee();
    }

    this.loadLeaves();
  }

  get f() {
    return this.leaveForm.controls;
  }

  get showApplyForm(): boolean {
    return this.role === 'Employee';
  }

  get canApprove(): boolean {
    return this.role === 'Admin' || this.role === 'Manager';
  }

  get canCancel(): boolean {
    return this.role === 'Employee';
  }

  private resolveCurrentEmployee(): void {
    if (!this.username) {
      return;
    }

    this.employeeService.search(this.username).subscribe({
      next: (response) => {
        const exact = (response.data ?? []).find((employee: any) =>
          String(employee.email ?? '').toLowerCase() === this.username.toLowerCase());

        this.currentEmployeeId = exact?.employeeId ?? 0;

        if (this.currentEmployeeId > 0) {
          this.leaveForm.patchValue({ empId: this.currentEmployeeId });
          this.leaveForm.get('empId')?.disable();
        }
      },
    });
  }

  loadLeaves(): void {
    this.loading = true;

    this.leaveService.getAll().subscribe({
      next: (response) => {
        this.leaves = response.data ?? [];
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  applyLeave(): void {
    if (this.leaveForm.invalid) {
      this.leaveForm.markAllAsTouched();
      return;
    }

    this.submitting = true;

    this.leaveService.apply(this.leaveForm.value).subscribe({
      next: () => {
        this.leaveForm.reset();
        this.submitting = false;
        this.loadLeaves();
      },
      error: () => {
        this.submitting = false;
      },
    });
  }

  approve(leaveId: number): void {
    if (!this.canApprove) {
      return;
    }

    this.leaveService.approve({
      leaveId,
      status: 2,
      approvedBy: this.authService.getUserId(),
    }).subscribe({
      next: () => this.loadLeaves(),
    });
  }

  reject(leaveId: number): void {
    if (!this.canApprove) {
      return;
    }

    this.leaveService.approve({
      leaveId,
      status: 3,
      approvedBy: this.authService.getUserId(),
    }).subscribe({
      next: () => this.loadLeaves(),
    });
  }

  cancel(leaveId: number): void {
    if (!this.canCancel) {
      return;
    }

    this.leaveService.cancel(leaveId).subscribe({
      next: () => this.loadLeaves(),
    });
  }
}
