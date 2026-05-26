import { Component, ElementRef, HostListener } from '@angular/core';
import { Router } from '@angular/router';
import { Auth } from '../../services/auth';
import { Employee } from '../../services/employee';

interface NavigationItem {
  label: string;
  description: string;
  route: string;
  roles: string[];
}

@Component({
  selector: 'app-navbar',

  standalone: false,

  templateUrl: './navbar.html',

  styleUrl: './navbar.css'
})

export class Navbar {

  role = '';

  accountMenuOpen = false;

  readonly navigationItems: NavigationItem[] = [
  { label: 'Dashboard', description: 'Overview and live metrics', route: '/dashboard', roles: ['Admin', 'Manager', 'Employee'] },
  { label: 'Employees', description: 'Roster and workforce records', route: '/employees', roles: ['Admin'] },
  { label: 'Attendance', description: 'Daily presence and reports', route: '/attendance', roles: ['Admin', 'Manager', 'Employee'] },
  { label: 'Leaves', description: 'Requests and approvals', route: '/leaves', roles: ['Admin', 'Manager', 'Employee'] },
  { label: 'Departments', description: 'Org structure and teams', route: '/departments', roles: ['Admin', 'Manager', 'Employee'] },
  { label: 'Clients', description: 'Accounts and relationships', route: '/clients', roles: ['Admin'] },
  { label: 'Projects', description: 'Delivery and assignments', route: '/projects', roles: ['Admin', 'Manager', 'Employee'] },
  { label: 'Announcements', description: 'Org updates and notices', route: '/announcements', roles: ['Admin', 'Manager', 'Employee'] },
  ];

  constructor(
    private authService:
      Auth,

    private employeeService: Employee,

    private router: Router,

    private elementRef: ElementRef<HTMLElement>
  )
  {
    this.role =
      this.authService.getRole();
  }

  get visibleNavigationItems(): NavigationItem[] {
    return this.navigationItems.filter((item) => item.roles.includes(this.role));
  }

  get showProfileOption(): boolean {
    return this.role === 'Manager' || this.role === 'Employee';
  }

  toggleAccountMenu(event: MouseEvent): void {
    event.stopPropagation();

    this.accountMenuOpen = !this.accountMenuOpen;
  }

  closeAccountMenu(): void {
    this.accountMenuOpen = false;
  }

  openProfile(): void {
    const username = this.authService.getUsername();

    this.closeAccountMenu();

    if (!this.showProfileOption || !username) {
      return;
    }

    this.employeeService.search(username).subscribe({
      next: (response) => {
        const employees = response.data ?? [];
        const normalizedUsername = username.toLowerCase();

        const exactMatch = employees.find((employee: any) =>
          String(employee.email ?? '').toLowerCase() === normalizedUsername
        );

        const profileEmployee = exactMatch ?? employees[0];

        if (profileEmployee?.employeeId) {
          this.router.navigate([
            '/employee-edit',
            profileEmployee.employeeId
          ]);
        }
      }
    });
  }

  openChangePassword(): void {
    this.closeAccountMenu();

    this.router.navigate([
      '/change-password'
    ]);
  }

  logout(): void {

    this.closeAccountMenu();

    this.authService.logout();

    this.router.navigate([
      '/login'
    ]);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as Node | null;

    if (target && this.elementRef.nativeElement.contains(target)) {
      return;
    }

    this.closeAccountMenu();
  }
}
