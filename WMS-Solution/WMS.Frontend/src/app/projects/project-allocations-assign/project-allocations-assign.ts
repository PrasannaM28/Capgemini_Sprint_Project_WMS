import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Allocation } from '../../services/allocation';
import { Auth } from '../../services/auth';
import { Project } from '../../services/project';
import { UiFeedbackService } from '../../shared/ui-feedback/ui-feedback.service';

@Component({
  selector: 'app-project-allocations-assign',
  standalone: false,
  templateUrl: './project-allocations-assign.html',
  styleUrl: './project-allocations-assign.css',
})
export class ProjectAllocationsAssign implements OnInit {
  allocationForm: FormGroup;

  loading = false;

  employees: any[] = [];

  projects: any[] = [];

  constructor(
    private fb: FormBuilder,
    private allocationService: Allocation,
    private projectService: Project,
    private authService: Auth,
    private router: Router,
    private feedback: UiFeedbackService
  ) {
    this.allocationForm = this.fb.group({
      empId: [null, Validators.required],
      projectId: [null, Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadAssignableEmployees();
    this.loadProjects();
  }

  get af() {
    return this.allocationForm.controls;
  }

  loadAssignableEmployees(): void {
    this.allocationService.getAssignableEmployees().subscribe({
      next: (response) => {
        this.employees = response.data ?? [];
      },
    });
  }

  loadProjects(): void {
    this.projectService.getAll().subscribe({
      next: (response) => {
        this.projects = response.data ?? [];
      },
    });
  }

  assign(): void {
    if (this.allocationForm.invalid) {
      this.allocationForm.markAllAsTouched();
      return;
    }

    this.loading = true;

    const payload = {
      ...this.allocationForm.value,
      createdBy: this.authService.getUsername(),
    };

    this.allocationService.allocate(payload).subscribe({
      next: (response) => {
        this.loading = false;
        this.feedback.success('Team member assigned', response?.message || 'Team member assigned successfully.');
        this.router.navigate(['/project-allocations']);
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  cancel(): void {
    this.router.navigate(['/project-allocations']);
  }
}