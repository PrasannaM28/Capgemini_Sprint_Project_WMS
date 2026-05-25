import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardHome } from './dashboard-home/dashboard-home';
import { SharedModule } from '../shared/shared-module';

@NgModule({
  declarations: [DashboardHome],
  imports: [CommonModule, SharedModule],
  exports: [DashboardHome]
})
export class DashboardModule {}
