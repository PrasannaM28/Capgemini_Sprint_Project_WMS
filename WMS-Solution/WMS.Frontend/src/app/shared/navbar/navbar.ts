import { Component, ElementRef, HostListener } from '@angular/core';
import { Router } from '@angular/router';
import { Auth } from '../../services/auth';
import { Employee } from '../../services/employee';

interface NavigationItem {
  label: string;
  description: string;
  route: string;
  roles: string[];
  icon: string;
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
    { label: 'Dashboard', description: 'Overview & metrics', route: '/dashboard', roles: ['Admin', 'Manager', 'Employee'], icon: 'bi-grid-1x2-fill' },
    { label: 'Employees', description: 'Roster & records', route: '/employees', roles: ['Admin'], icon: 'bi-people-fill' },
    { label: 'Attendance', description: 'Daily presence', route: '/attendance', roles: ['Admin', 'Manager', 'Employee'], icon: 'bi-clock-fill' },
    { label: 'Leaves', description: 'Requests & approvals', route: '/leaves', roles: ['Admin', 'Manager', 'Employee'], icon: 'bi-calendar-check-fill' },
    { label: 'Departments', description: 'Org structure', route: '/departments', roles: ['Admin', 'Manager', 'Employee'], icon: 'bi-building' },
    { label: 'Clients', description: 'Accounts & relationships', route: '/clients', roles: ['Admin'], icon: 'bi-person-badge-fill' },
    { label: 'Projects', description: 'Delivery & tracking', route: '/projects', roles: ['Admin', 'Manager', 'Employee'], icon: 'bi-folder-fill' },
    { label: 'Allocations', description: 'Team assignments', route: '/project-allocations', roles: ['Admin', 'Manager'], icon: 'bi-diagram-3-fill' },
    { label: 'Announcements', description: 'Org updates', route: '/announcements', roles: ['Admin', 'Manager', 'Employee'], icon: 'bi-megaphone-fill' },
  ];

  constructor(
    private authService: Auth,
    private employeeService: Employee,
    private router: Router,
    private elementRef: ElementRef<HTMLElement>
  ) {
    this.role = this.authService.getRole();
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
    if (!this.showProfileOption || !username) return;
    this.employeeService.search(username).subscribe({
      next: (response) => {
        const employees = response.data ?? [];
        const match = employees.find((e: any) => String(e.email ?? '').toLowerCase() === username.toLowerCase());
        const profile = match ?? employees[0];
        if (profile?.employeeId) this.router.navigate(['/employee-edit', profile.employeeId]);
      }
    });
  }

  openChangePassword(): void {
    this.closeAccountMenu();
    this.router.navigate(['/change-password']);
  }

  logout(): void {
    this.closeAccountMenu();
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as Node | null;
    if (target && this.elementRef.nativeElement.contains(target)) return;
    this.closeAccountMenu();
  }
}
