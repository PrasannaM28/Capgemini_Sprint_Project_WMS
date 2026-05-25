import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmployeeList } from './employee-list/employee-list';
import { EmployeeCreate } from './employee-create/employee-create';
import { EmployeeEdit } from './employee-edit/employee-edit';
import { SharedModule } from '../shared/shared-module';
import {
  ReactiveFormsModule
}
from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [EmployeeList, EmployeeCreate, EmployeeEdit],
  imports: [CommonModule, SharedModule, ReactiveFormsModule, FormsModule, RouterModule],
  exports: [EmployeeList, EmployeeCreate, EmployeeEdit]
})
export class EmployeesModule {}
