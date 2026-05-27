import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Department } from '../../services/department';
import { UiFeedbackService } from '../../shared/ui-feedback/ui-feedback.service';

@Component({
  selector: 'app-department-form',
  standalone: false,
  templateUrl: './department-form.html',
  styleUrl: './department-form.css',
})
export class DepartmentForm implements OnInit {
  departmentForm: FormGroup;

  loading = false;

  departmentId = 0;

  constructor(
    private fb: FormBuilder,
    private departmentService: Department,
    private route: ActivatedRoute,
    private router: Router,
    private feedback: UiFeedbackService
  ) {
    this.departmentForm = this.fb.group({
      departmentName: ['', Validators.required],
      description: [''],
    });
  }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (id) {
      this.departmentId = id;
      this.loadDepartment();
    }
  }

  get isEditMode(): boolean {
    return this.departmentId > 0;
  }

  get pageHeading(): string {
    return this.isEditMode ? 'Edit Department' : 'Add Department';
  }

  get f() {
    return this.departmentForm.controls;
  }

  loadDepartment(): void {
    this.loading = true;

    this.departmentService.getAll().subscribe({
      next: (response) => {
        const departments = response.data ?? [];
        const department = departments.find((item: any) => Number(item.departmentId) === this.departmentId);

        if (department) {
          this.departmentForm.patchValue({
            departmentName: department.departmentName,
            description: department.description ?? '',
          });
        }

        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  saveDepartment(): void {
    if (this.departmentForm.invalid) {
      this.departmentForm.markAllAsTouched();
      return;
    }

    this.loading = true;

    const payload = this.departmentForm.value;
    const request = this.isEditMode
      ? this.departmentService.update(this.departmentId, payload)
      : this.departmentService.create(payload);

    request.subscribe({
      next: () => {
        this.feedback.success(
          this.isEditMode ? 'Department updated' : 'Department created',
          this.isEditMode ? 'The department was updated successfully.' : 'The department was created successfully.'
        );

        this.router.navigate(['/departments']);
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  cancel(): void {
    this.router.navigate(['/departments']);
  }
}