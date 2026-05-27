import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Auth } from '../../services/auth';
import { Department } from '../../services/department';
import { UiFeedbackService } from '../../shared/ui-feedback/ui-feedback.service';

@Component({
  selector: 'app-department-management',
  standalone: false,
  templateUrl: './department-management.html',
  styleUrl: './department-management.css',
})
export class DepartmentManagement implements OnInit {
  departments: any[] = [];

  loading = false;

  role = '';

  constructor(
    private departmentService: Department,
    private authService: Auth,
    private router: Router,
    private feedback: UiFeedbackService
  ) {}

  ngOnInit(): void {
    this.role = this.authService.getRole();
    this.loadDepartments();
  }

  get canManageDepartments(): boolean {
    return this.role === 'Admin';
  }

  loadDepartments(): void {
    this.loading = true;

    this.departmentService.getAll().subscribe({
      next: (response) => {
        this.departments = response.data ?? [];
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  addDepartment(): void {
    if (!this.canManageDepartments) {
      return;
    }

    this.router.navigate(['/departments/add']);
  }

  editDepartment(department: any): void {
    if (!this.canManageDepartments) {
      return;
    }

    this.router.navigate(['/departments/edit', department.departmentId]);
  }

  deleteDepartment(id: number): void {
    if (!this.canManageDepartments) {
      return;
    }

    this.feedback.confirm({
      title: 'Delete department?',
      message: 'This department will be removed from the organization view.',
      confirmLabel: 'Delete',
      tone: 'warning',
    }).subscribe((accepted) => {
      if (!accepted) {
        return;
      }

      this.loading = true;

      this.departmentService.delete(id).subscribe({
        next: () => {
          this.feedback.success('Department deleted', 'The department was removed successfully.');
          this.loadDepartments();
        },
        error: () => {
          this.loading = false;
        },
      });
    });
  }
}
