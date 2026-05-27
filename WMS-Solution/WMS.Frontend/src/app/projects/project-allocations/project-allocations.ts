import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { Allocation } from '../../services/allocation';
import { Auth } from '../../services/auth';
import { Project } from '../../services/project';
import { UiFeedbackService } from '../../shared/ui-feedback/ui-feedback.service';

@Component({
  selector: 'app-project-allocations',
  standalone: false,
  templateUrl: './project-allocations.html',
  styleUrl: './project-allocations.css',
})
export class ProjectAllocations implements OnInit {
  projectAllocations: any[] = [];

  searchForm: FormGroup;

  projects: any[] = [];

  loading = false;

  role = '';

  constructor(
    private fb: FormBuilder,
    private projectService: Project,
    private allocationService: Allocation,
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
    this.loadProjectAllocations();
  }

  get canAssignTeam(): boolean {
    return this.role === 'Admin' || this.role === 'Manager';
  }

  assignTeam(): void {
    if (!this.canAssignTeam) {
      return;
    }

    this.router.navigate(['/project-allocations/assign']);
  }

  loadProjectAllocations(): void {
    this.loading = true;

    this.projectService.getAll().subscribe({
      next: (response) => {
        this.projects = response.data ?? [];

        if (!this.projects.length) {
          this.projectAllocations = [];
          this.loading = false;
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
            ).sort((left: any, right: any) =>
              new Date(right.assignedOn ?? 0).getTime() - new Date(left.assignedOn ?? 0).getTime()
            );
            this.loading = false;
          },
          error: () => {
            this.loading = false;
          },
        });
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  get filteredProjectAllocations(): any[] {
    const term = String(this.searchForm.value.searchTerm ?? '').trim().toLowerCase();

    if (!term) {
      return this.projectAllocations;
    }

    return this.projectAllocations.filter(allocation =>
      String(allocation.employeeName ?? '').toLowerCase().includes(term) ||
      String(allocation.projectName ?? '').toLowerCase().includes(term)
    );
  }

  search(): void {
    this.projectAllocations = [...this.projectAllocations];
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
}