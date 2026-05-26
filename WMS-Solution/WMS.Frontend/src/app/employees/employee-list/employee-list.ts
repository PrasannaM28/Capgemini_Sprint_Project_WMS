import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

import { Employee } from '../../services/employee';
import { Auth } from '../../services/auth';
import { UiFeedbackService } from '../../shared/ui-feedback/ui-feedback.service';

@Component({
  selector: 'app-employee-list',
  standalone: false,
  templateUrl: './employee-list.html',
  styleUrl: './employee-list.css'
})
export class EmployeeList implements OnInit {
  employees: any[] = [];
  pageNumber = 1;
  pageSize = 5;
  totalRecords = 0;
  loading = false;
  searchForm: FormGroup;
  role = '';
  username = '';
  currentEmployeeId = 0;

  constructor(
    private employeeService: Employee,
    private authService: Auth,
    private feedback: UiFeedbackService,
    private fb: FormBuilder
  ) {
    this.searchForm = this.fb.group({
      searchTerm: ['']
    });
  }

  ngOnInit(): void {
    this.role = this.authService.getRole();
    this.username = this.authService.getUsername();

    if (this.role === 'Employee') {
      this.resolveCurrentEmployee();
    }

    this.loadEmployees();
  }

  get canCreate(): boolean {
    return this.role === 'Admin';
  }

  canEdit(employeeId: number): boolean {
    if (this.role === 'Admin' || this.role === 'Manager') {
      return true;
    }

    return this.role === 'Employee' && this.currentEmployeeId === employeeId;
  }

  get canDelete(): boolean {
    return this.role === 'Admin';
  }

  private resolveCurrentEmployee(): void {
    if (!this.username) {
      return;
    }

    this.employeeService.search(this.username).subscribe({
      next: (response) => {
        const exact = (response.data ?? []).find((employee: any) =>
          String(employee.email ?? '').toLowerCase() === this.username.toLowerCase()
        );

        this.currentEmployeeId = exact?.employeeId ?? 0;
      }
    });
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalRecords / this.pageSize));
  }

  loadEmployees(): void {
    this.loading = true;

    this.employeeService.getPaged(this.pageNumber, this.pageSize).subscribe({
      next: (response) => {
        this.loading = false;
        this.employees = response.data.items;
        this.totalRecords = response.data.totalRecords;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  search(): void {
    const term = this.searchForm.value.searchTerm;

    if (!term.trim()) {
      this.pageNumber = 1;
      this.loadEmployees();
      return;
    }

    this.pageNumber = 1;
    this.loading = true;

    this.employeeService.search(term).subscribe({
      next: (response) => {
        this.loading = false;
        this.employees = response.data;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  nextPage(): void {
    if (this.loading || this.pageNumber >= this.totalPages) {
      return;
    }

    this.pageNumber++;
    this.loadEmployees();
  }

  previousPage(): void {
    if (this.loading || this.pageNumber <= 1) {
      return;
    }

    this.pageNumber--;
    this.loadEmployees();
  }

  delete(id: number): void {
    this.feedback.confirm({
      title: 'Delete employee?',
      message: 'This employee record will be removed permanently.',
      confirmLabel: 'Delete',
      tone: 'warning'
    }).subscribe((accepted) => {
      if (!accepted) {
        return;
      }

      this.loading = true;

      this.employeeService.delete(id).subscribe({
        next: () => {
          this.loading = false;
          this.feedback.success('Employee deleted', 'The employee record was removed.');
          this.loadEmployees();
        },
        error: () => {
          this.loading = false;
        }
      });
    });
  }
}