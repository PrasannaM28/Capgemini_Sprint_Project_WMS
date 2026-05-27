import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { Auth } from '../../services/auth';
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

  searchForm: FormGroup;

  loading = false;

  role = '';

  constructor(
    private fb: FormBuilder,
    private projectService: Project,
    private authService: Auth,
    private router: Router,
    private feedback: UiFeedbackService
  ) {
    this.searchForm = this.fb.group({
      searchTerm: ['']
    });
  }

  ngOnInit(): void {
    this.role = this.authService.getRole();

    this.loadProjects();
  }

  get canManageProjects(): boolean {
    return this.role === 'Admin';
  }

  loadProjects(): void {
    this.loading = true;

    this.projectService.getAll().subscribe({
      next: (response) => {
        this.projects = response.data ?? [];
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  get filteredProjects(): any[] {
    const term = String(this.searchForm.value.searchTerm ?? '').trim().toLowerCase();

    if (!term) {
      return this.projects;
    }

    return this.projects.filter(project =>
      String(project.projectName ?? '').toLowerCase().includes(term) ||
      String(project.clientName ?? '').toLowerCase().includes(term)
    );
  }

  search(): void {
    this.projects = [...this.projects];
  }

  addProject(): void {
    if (!this.canManageProjects) {
      return;
    }

    this.router.navigate(['/projects/add']);
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
}
