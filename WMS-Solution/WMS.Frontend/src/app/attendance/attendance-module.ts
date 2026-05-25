import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AttendanceList } from './attendance-list/attendance-list';
import { SharedModule } from '../shared/shared-module';
import {
  ReactiveFormsModule
}
from '@angular/forms';

@NgModule({
  declarations: [AttendanceList],
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  exports: [AttendanceList]
})
export class AttendanceModule {}
