import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { Login } from './login/login';
import { SharedModule } from '../shared/shared-module';

@NgModule({
  declarations: [Login],
  imports: [CommonModule, ReactiveFormsModule, SharedModule],
  exports: [Login],
})
export class AuthModule {}
