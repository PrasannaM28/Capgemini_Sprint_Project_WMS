import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../shared/shared-module';
import { ClientManagement } from './client-management/client-management';
import { ClientForm } from './client-form/client-form';

@NgModule({
  declarations: [ClientManagement, ClientForm],
  imports: [CommonModule, ReactiveFormsModule, RouterModule, SharedModule],
  exports: [ClientManagement, ClientForm],
})
export class ClientsModule {}