import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LeaveList } from './leave-list/leave-list';
import { LeaveApply } from './leave-apply/leave-apply';
import { SharedModule } from '../shared/shared-module';
import { ReactiveFormsModule } from '@angular/forms';  

@NgModule({
  declarations: [LeaveList, LeaveApply],
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  exports: [LeaveList, LeaveApply]
})
export class LeavesModule {}
