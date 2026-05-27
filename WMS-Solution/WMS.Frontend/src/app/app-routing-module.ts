import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { Login } from './auth/login/login';
import { DashboardHome } from './dashboard/dashboard-home/dashboard-home';
import { EmployeeList } from './employees/employee-list/employee-list';
import { AuthGuard } from './guards/auth-guard';
import { EmployeeCreate } from './employees/employee-create/employee-create';
import { AttendanceList } from './attendance/attendance-list/attendance-list';
import { LeaveApply } from './leaves/leave-apply/leave-apply';
import { LeaveList } from './leaves/leave-list/leave-list';
import { EmployeeEdit } from './employees/employee-edit/employee-edit';
import { AnnouncementList } from './announcement/announcement-list/announcement-list';
import { DepartmentManagement } from './departments/department-management/department-management';
import { DepartmentForm } from './departments/department-form/department-form';
import { ClientManagement } from './clients/client-management/client-management';
import { ClientForm } from './clients/client-form/client-form';
import { ProjectManagement } from './projects/project-management/project-management';
import { ProjectForm } from './projects/project-form/project-form';
import { ProjectAllocations } from './projects/project-allocations/project-allocations';
import { ProjectAllocationsAssign } from './projects/project-allocations-assign/project-allocations-assign';
import { ChangePassword } from './auth/change-password/change-password';
import { RoleGuard } from './guards/role-guard';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
  {
    path: 'login',
    component: Login,
  },
  {
    path: 'dashboard',
    component: DashboardHome,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'Manager', 'Employee'] },
  },
  {
    path: 'employees',
    component: EmployeeList,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'Manager', 'Employee'] },
  },
  {
    path: 'employee-create',
    component: EmployeeCreate,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'attendance',
    component: AttendanceList,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'Manager', 'Employee'] },
  },
  {
    path: 'leaves',
    component: LeaveList,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'Manager', 'Employee'] },
  },
  {
    path: 'leave-apply',
    component: LeaveApply,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Employee'] },
  },
  {
    path: 'departments/add',
    component: DepartmentForm,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'departments/edit/:id',
    component: DepartmentForm,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'clients/add',
    component: ClientForm,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'clients/edit/:id',
    component: ClientForm,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'project-allocations/assign',
    component: ProjectAllocationsAssign,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'Manager'] },
  },
  {
    path: 'project-allocations',
    component: ProjectAllocations,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'Manager', 'Employee'] },
  },
  {
    path: 'projects/add',
    component: ProjectForm,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'projects/edit/:id',
    component: ProjectForm,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'projects',
    component: ProjectManagement,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'Manager', 'Employee'] },
  },
  {
    path: 'departments',
    component: DepartmentManagement,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'Manager', 'Employee'] },
  },
  {
    path: 'clients',
    component: ClientManagement,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'employee-edit/:id',
    component: EmployeeEdit,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'Manager', 'Employee'] },
  },
  {
    path: 'announcements',
    component: AnnouncementList,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'Manager', 'Employee'] },
  },
  {
    path: 'change-password',
    component: ChangePassword,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'Manager', 'Employee'] },
  },
  {
    path: '**',
    redirectTo: 'login',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
