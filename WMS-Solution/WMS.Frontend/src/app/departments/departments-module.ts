import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared-module';
import { DepartmentManagement } from './department-management/department-management';
import { DepartmentForm } from './department-form/department-form';

@NgModule({
  declarations: [DepartmentManagement, DepartmentForm],
  imports: [CommonModule, ReactiveFormsModule, RouterModule, SharedModule],
  exports: [DepartmentManagement, DepartmentForm],
})
export class DepartmentsModule {}
