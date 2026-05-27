import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Client } from '../../services/client';
import { Project } from '../../services/project';
import { UiFeedbackService } from '../../shared/ui-feedback/ui-feedback.service';

@Component({
  selector: 'app-project-form',
  standalone: false,
  templateUrl: './project-form.html',
  styleUrl: './project-form.css',
})
export class ProjectForm implements OnInit {
  projectForm: FormGroup;

  loading = false;

  projectId = 0;

  clients: any[] = [];

  readonly statusOptions = [
    { value: 1, label: 'Active' },
    { value: 2, label: 'Completed' },
    { value: 3, label: 'On Hold' },
  ];

  constructor(
    private fb: FormBuilder,
    private projectService: Project,
    private clientService: Client,
    private route: ActivatedRoute,
    private router: Router,
    private feedback: UiFeedbackService
  ) {
    this.projectForm = this.fb.group({
      projectName: ['', Validators.required],
      clientId: [null],
      startDate: [''],
      endDate: [''],
      status: [1, Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadClients();

    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (id) {
      this.projectId = id;
      this.loadProject();
    }
  }

  get isEditMode(): boolean {
    return this.projectId > 0;
  }

  get pageHeading(): string {
    return this.isEditMode ? 'Edit Project' : 'Add Project';
  }

  get f() {
    return this.projectForm.controls;
  }

  loadClients(): void {
    this.clientService.getAll().subscribe({
      next: (response) => {
        this.clients = response.data ?? [];
      },
    });
  }

  loadProject(): void {
    this.loading = true;

    this.projectService.getAll().subscribe({
      next: (response: any) => {
        const projects = response.data ?? [];
        const project = projects.find((item: any) => Number(item.projectId) === this.projectId);

        if (project) {
          this.projectForm.patchValue({
            projectName: project.projectName,
            clientId: project.clientId,
            startDate: this.toDateInput(project.startDate),
            endDate: this.toDateInput(project.endDate),
            status: this.toStatusValue(project.status),
          });
        }

        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  saveProject(): void {
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

    const request = this.isEditMode
      ? this.projectService.update(this.projectId, payload)
      : this.projectService.create(payload);

    request.subscribe({
      next: () => {
        this.feedback.success(
          this.isEditMode ? 'Project updated' : 'Project created',
          this.isEditMode ? 'The project was updated successfully.' : 'The project was created successfully.'
        );

        this.router.navigate(['/projects']);
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  cancel(): void {
    this.router.navigate(['/projects']);
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