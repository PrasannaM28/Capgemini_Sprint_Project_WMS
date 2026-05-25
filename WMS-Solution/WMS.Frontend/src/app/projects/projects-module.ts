import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared-module';
import { ProjectManagement } from './project-management/project-management';

@NgModule({
  declarations: [ProjectManagement],
  imports: [CommonModule, ReactiveFormsModule, SharedModule],
  exports: [ProjectManagement],
})
export class ProjectsModule {}
