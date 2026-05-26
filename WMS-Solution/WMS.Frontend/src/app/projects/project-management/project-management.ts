import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { Auth } from '../../services/auth';
import { Allocation } from '../../services/allocation';
import { Client } from '../../services/client';
import { Project } from '../../services/project';
import { UiFeedbackService } from '../../shared/ui-feedback/ui-feedback.service';

@Component({
  selector: 'app-project-management',
  standalone: false,
  templateUrl: './project-management.html',
  styleUrl: './project-management.css',
})
export class ProjectManagement implements OnInit {
  projects: any[] = [];

  projectForm: FormGroup;

  allocationForm: FormGroup;

  loading = false;

  editingProjectId: number | null = null;

  role = '';

  employees: any[] = [];

  clients: any[] = [];

  projectAllocations: any[] = [];

  readonly statusOptions = [
    { value: 1, label: 'Active' },
    { value: 2, label: 'Completed' },
    { value: 3, label: 'On Hold' },
  ];

  constructor(
    private fb: FormBuilder,
    private projectService: Project,
    private authService: Auth,
    private allocationService: Allocation,
    private clientService: Client,
    private feedback: UiFeedbackService
  ) {
    this.projectForm = this.fb.group({
      projectName: ['', Validators.required],
      clientId: [null],
      startDate: [''],
      endDate: [''],
      status: [1, Validators.required],
    });

    this.allocationForm = this.fb.group({
      empId: [null, Validators.required],
      projectId: [null, Validators.required],
    });
  }

  ngOnInit(): void {
    this.role = this.authService.getRole();

    this.loadClients();

    if (this.canAssignTeam) {
      this.loadEmployees();
    }

    this.loadProjects();
  }

  get canManageProjects(): boolean {
    return this.role === 'Admin';
  }

  get canAssignTeam(): boolean {
    return this.role === 'Admin' || this.role === 'Manager';
  }

  get f() {
    return this.projectForm.controls;
  }

  get af() {
    return this.allocationForm.controls;
  }

  loadEmployees(): void {
    this.allocationService.getAssignableEmployees().subscribe({
      next: (response) => {
        this.employees = response.data ?? [];
      },
    });
  }

  loadClients(): void {
    this.clientService.getAll().subscribe({
      next: (response) => {
        this.clients = response.data ?? [];
      },
    });
  }

  loadProjects(): void {
    this.loading = true;

    this.projectService.getAll().subscribe({
      next: (response) => {
        this.projects = response.data ?? [];
        this.loading = false;
        this.loadProjectAllocations();
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  loadProjectAllocations(): void {
    if (!this.canAssignTeam || !this.projects.length) {
      this.projectAllocations = [];
      return;
    }

    forkJoin(
      this.projects.map((project) =>
        this.allocationService.getProjectAllocations(project.projectId)
      )
    ).subscribe({
      next: (responses) => {
        this.projectAllocations = responses.flatMap((response: any) =>
          (response.data ?? []).map((allocation: any) => allocation)
        );
      },
    });
  }

  editProject(project: any): void {
    if (!this.canManageProjects) {
      return;
    }

    this.editingProjectId = project.projectId;
    this.projectForm.patchValue({
      projectName: project.projectName,
      clientId: project.clientId,
      startDate: this.toDateInput(project.startDate),
      endDate: this.toDateInput(project.endDate),
      status: this.toStatusValue(project.status),
    });
  }

  cancelEdit(): void {
    this.editingProjectId = null;
    this.projectForm.reset();
  }

  saveProject(): void {
    if (!this.canManageProjects) {
      return;
    }

    if (this.projectForm.invalid) {
      this.projectForm.markAllAsTouched();
      return;
    }

    const payload = {
      ...this.projectForm.value,
      clientId: this.projectForm.value.clientId || null,
      status: Number(this.projectForm.value.status),
    };

    this.loading = true;

    const request = this.editingProjectId
      ? this.projectService.update(this.editingProjectId, payload)
      : this.projectService.create(payload);

    request.subscribe({
      next: () => {
        this.projectForm.reset();
        this.editingProjectId = null;
        this.loadProjects();
        this.feedback.success('Project saved', 'The project board has been updated.');
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  deleteProject(id: number): void {
    if (!this.canManageProjects) {
      return;
    }

    this.feedback.confirm({
      title: 'Delete project?',
      message: 'This project will be removed from the workspace.',
      confirmLabel: 'Delete',
      tone: 'warning',
    }).subscribe((accepted) => {
      if (!accepted) {
        return;
      }

      this.loading = true;

      this.projectService.delete(id).subscribe({
        next: () => {
          this.feedback.success('Project deleted', 'The project was removed successfully.');
          this.loadProjects();
        },
        error: () => {
          this.loading = false;
        },
      });
    });
  }

  assignTeamMember(): void {
    if (!this.canAssignTeam) {
      return;
    }

    if (this.allocationForm.invalid) {
      this.allocationForm.markAllAsTouched();
      return;
    }

    const payload = {
      ...this.allocationForm.value,
      createdBy: this.authService.getUsername(),
    };

    this.allocationService.allocate(payload).subscribe({
      next: (response) => {
        this.allocationForm.reset();
        this.feedback.success('Team member assigned', response?.message || 'Team member assigned successfully.');
        this.loadProjectAllocations();
      },
    });
  }

  deassignMember(allocation: any): void {
    if (!this.canAssignTeam) {
      return;
    }

    this.feedback.confirm({
      title: 'Remove team member?',
      message: `Deassign ${allocation.employeeName} from ${allocation.projectName}?`,
      confirmLabel: 'Remove',
      tone: 'warning',
    }).subscribe((accepted) => {
      if (!accepted) {
        return;
      }

      this.allocationService.deassign(allocation.projectId, allocation.empId).subscribe({
        next: (response) => {
          this.feedback.success('Team member removed', response?.message || 'Team member deassigned successfully.');
          this.loadProjectAllocations();
        },
      });
    });
  }

  private toStatusValue(value: unknown): number {
    if (typeof value === 'number') {
      return value;
    }

    if (typeof value === 'string') {
      const normalized = value.toLowerCase().replace(/\s+/g, '');

      if (normalized === 'completed') {
        return 2;
      }

      if (normalized === 'onhold') {
        return 3;
      }
    }

    return 1;
  }

  private toDateInput(value: string | Date | null | undefined): string {
    if (!value) {
      return '';
    }

    const date = new Date(value);
    return date.toISOString().slice(0, 10);
  }
}
