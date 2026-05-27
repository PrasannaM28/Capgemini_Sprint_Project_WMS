import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared-module';
import { ProjectManagement } from './project-management/project-management';
import { ProjectForm } from './project-form/project-form';
import { ProjectAllocations } from './project-allocations/project-allocations';
import { ProjectAllocationsAssign } from './project-allocations-assign/project-allocations-assign';

@NgModule({
  declarations: [ProjectManagement, ProjectForm, ProjectAllocations, ProjectAllocationsAssign],
  imports: [CommonModule, ReactiveFormsModule, RouterModule, SharedModule],
  exports: [ProjectManagement, ProjectForm, ProjectAllocations, ProjectAllocationsAssign],
})
export class ProjectsModule {}
