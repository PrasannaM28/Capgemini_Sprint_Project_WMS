import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Navbar } from './navbar/navbar';
import { RouterModule } from '@angular/router';
import { Loader } from './loader/loader';

@NgModule({
  declarations: [Navbar, Loader],
  imports: [CommonModule, RouterModule],
  exports: [Navbar, Loader],
})
export class SharedModule {}
