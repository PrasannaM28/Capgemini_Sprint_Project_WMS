import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { AuthModule } from './auth/auth-module';
import { DashboardModule } from './dashboard/dashboard-module';
import { EmployeesModule } from './employees/employees-module';
import { AttendanceModule } from './attendance/attendance-module';
import { LeavesModule } from './leaves/leaves-module';
import { DepartmentsModule } from './departments/departments-module';
import { ClientManagement } from './clients/client-management/client-management';
import { ProjectsModule } from './projects/projects-module';
import { AnnouncementModule } from './announcement/announcement-module';
import { SharedModule } from './shared/shared-module';
import { AuthInterceptor } from './interceptors/auth-interceptor';
import { ErrorInterceptor } from './interceptors/error-interceptor';
import { ChangeDetectionInterceptor } from './interceptors/change-detection.interceptor';

@NgModule({
  declarations: [App, ClientManagement],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    AuthModule,
    DashboardModule,
    EmployeesModule,
    AttendanceModule,
    LeavesModule,
    DepartmentsModule,
    ProjectsModule,
    AnnouncementModule,
    SharedModule,
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ChangeDetectionInterceptor,
      multi: true,
    },
  ],
  bootstrap: [App],
})
export class AppModule {}
