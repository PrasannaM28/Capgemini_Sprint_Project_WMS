import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
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

  departmentForm: FormGroup;

  loading = false;

  editingDepartmentId: number | null = null;

  role = '';

  constructor(
    private fb: FormBuilder,
    private departmentService: Department,
    private authService: Auth,
    private feedback: UiFeedbackService
  ) {
    this.departmentForm = this.fb.group({
      departmentName: ['', Validators.required],
      description: [''],
    });
  }

  ngOnInit(): void {
    this.role = this.authService.getRole();
    this.loadDepartments();
  }

  get canManageDepartments(): boolean {
    return this.role === 'Admin';
  }

  get f() {
    return this.departmentForm.controls;
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

  editDepartment(department: any): void {
    if (!this.canManageDepartments) {
      return;
    }

    this.editingDepartmentId = department.departmentId;
    this.departmentForm.patchValue({
      departmentName: department.departmentName,
      description: department.description ?? '',
    });
  }

  cancelEdit(): void {
    this.editingDepartmentId = null;
    this.departmentForm.reset();
  }

  saveDepartment(): void {
    if (!this.canManageDepartments) {
      return;
    }

    if (this.departmentForm.invalid) {
      this.departmentForm.markAllAsTouched();
      return;
    }

    const payload = this.departmentForm.value;
    this.loading = true;

    const request = this.editingDepartmentId
      ? this.departmentService.update(this.editingDepartmentId, payload)
      : this.departmentService.create(payload);

    request.subscribe({
      next: () => {
        this.departmentForm.reset();
        this.editingDepartmentId = null;
        this.loadDepartments();
        this.feedback.success('Department saved', 'The department list has been refreshed.');
      },
      error: () => {
        this.loading = false;
      },
    });
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
