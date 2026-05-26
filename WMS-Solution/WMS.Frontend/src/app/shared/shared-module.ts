import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Navbar } from './navbar/navbar';
import { RouterModule } from '@angular/router';
import { Loader } from './loader/loader';
import { UiFeedbackHost } from './ui-feedback/ui-feedback-host';

@NgModule({
  declarations: [Navbar, Loader, UiFeedbackHost],
  imports: [CommonModule, RouterModule],
  exports: [Navbar, Loader, UiFeedbackHost],
})
export class SharedModule {}
