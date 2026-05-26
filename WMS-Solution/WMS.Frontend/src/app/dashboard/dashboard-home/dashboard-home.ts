import {
  Component,
  OnDestroy,
  OnInit
}
from '@angular/core';

import {
  NavigationEnd,
  Router
}
from '@angular/router';

import {
  Chart,
  registerables
}
from 'chart.js';

import { filter, Subscription } from 'rxjs';

import { Auth }
from '../../services/auth';
import { Dashboard }
from '../../services/dashboard';

Chart.register(
  ...registerables
);

@Component({
  selector: 'app-dashboard-home',
  standalone: false,
  templateUrl: './dashboard-home.html',
  styleUrl: './dashboard-home.css',
})

export class DashboardHome
implements OnInit, OnDestroy {

  dashboardData: any;

  role = '';

  private attendanceTrendChart?: Chart;

  private projectPieChart?: Chart;

  private routerSubscription?: Subscription;

  constructor(
    private dashboardService: Dashboard,
    private authService: Auth,
    private router: Router
  ) { }

  ngOnInit(): void {

    this.role = this.authService.getRole();

    this.loadDashboard();

    this.routerSubscription =
      this.router.events
        .pipe(
          filter((event): event is NavigationEnd =>
            event instanceof NavigationEnd
          )
        )
        .subscribe(event =>
        {
          if (event.urlAfterRedirects === '/dashboard')
          {
            this.loadDashboard();
          }
        });
  }

  ngOnDestroy(): void {

    this.destroyCharts();

    this.routerSubscription?.unsubscribe();
  }

  loadDashboard(): void {

    this.dashboardService
      .getDashboard()
      .subscribe({

        next: (response) =>
        {
          this.destroyCharts();

          this.dashboardData =
            response.data;

          setTimeout(() =>
          {
            this.loadAdminCharts();
          });
        }
      });
  }

  get isAdmin(): boolean {
    return this.role === 'Admin';
  }

  get isManager(): boolean {
    return this.role === 'Manager';
  }

  get isEmployee(): boolean {
    return this.role === 'Employee';
  }

  get adminEmployeeCards(): Array<{ label: string; value: number }> {

    const data = this.dashboardData ?? {};

    return [
      { label: 'Total Employees', value: data.totalEmployees ?? 0 },
      { label: 'Present Today', value: data.presentToday ?? 0 },
      { label: 'Absent Today', value: data.absentToday ?? 0 },
    ];
  }

  get adminLeaveCards(): Array<{ label: string; value: number }> {

    const data = this.dashboardData ?? {};

    return [
      { label: 'Pending', value: data.pendingLeaves ?? 0 },
      { label: 'Approved', value: data.approvedLeaves ?? 0 },
      { label: 'Rejected', value: data.rejectedLeaves ?? 0 },
    ];
  }

  get adminBottomCards(): Array<{ label: string; value: number }> {

    const data = this.dashboardData ?? {};

    return [
      { label: 'Total Departments', value: data.totalDepartments ?? 0 },
      { label: 'Total Clients', value: data.totalClients ?? 0 },
    ];
  }

  get managerCards(): Array<{ label: string; value: number }> {

    const data = this.dashboardData ?? {};

    return [
      { label: 'Team Members', value: data.totalEmployees ?? 0 },
      { label: 'Projects', value: data.totalProjects ?? 0 },
      { label: 'Pending Leaves', value: data.pendingLeaves ?? 0 },
      { label: 'Announcements', value: data.activeAnnouncements ?? 0 },
    ];
  }

  get employeeCards(): Array<{ label: string; value: number }> {

    const data = this.dashboardData ?? {};

    return [
      { label: 'Projects', value: data.totalProjects ?? 0 },
      { label: 'Pending Leaves', value: data.pendingLeaves ?? 0 },
      { label: 'Announcements', value: data.activeAnnouncements ?? 0 },
    ];
  }

  get ownAttendancePercent(): number {
    const value = Number(this.dashboardData?.ownAttendancePercent ?? 0);
    return Math.max(0, Math.min(100, value));
  }

  get ownAttendanceProgressLabel(): string {
    const presentDays = Number(this.dashboardData?.ownPresentDays ?? 0);
    const targetDays = Number(this.dashboardData?.ownAttendanceTargetDays ?? 0);

    if (targetDays <= 0)
    {
      return 'No data';
    }

    return `${presentDays}/${targetDays} days`;
  }

  get ringStrokeDashArray(): string {
    const circumference = 2 * Math.PI * 52;
    return `${circumference} ${circumference}`;
  }

  get ringStrokeDashOffset(): number {
    const circumference = 2 * Math.PI * 52;
    return circumference - (this.ownAttendancePercent / 100) * circumference;
  }

  private loadAdminCharts(): void {

    if (!this.isAdmin)
    {
      return;
    }

    const trend = this.dashboardData?.attendanceLast7Days ?? [];

    this.attendanceTrendChart = new Chart(
      'attendanceTrendChart',
      {
        type: 'line',
        data: {
          labels: trend.map((point: any) => this.formatDateLabel(point.date)),
          datasets: [
            {
              label: 'Present Count',
              data: trend.map((point: any) => Number(point.presentCount ?? 0)),
              borderColor: '#0d6efd',
              backgroundColor: 'rgba(13, 110, 253, 0.2)',
              borderWidth: 2,
              tension: 0.3,
              fill: true,
            }
          ]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            x: {
              title: {
                display: true,
                text: 'Date'
              }
            },
            y: {
              beginAtZero: true,
              title: {
                display: true,
                text: 'Present Count'
              },
              ticks: {
                precision: 0
              }
            }
          }
        }
      }
    );

    this.projectPieChart = new Chart(
      'projectPieChart',
      {
        type: 'pie',
        data: {
          labels: ['Active', 'Completed'],
          datasets: [
            {
              data: [
                Number(this.dashboardData?.activeProjects ?? 0),
                Number(this.dashboardData?.completedProjects ?? 0)
              ],
              backgroundColor: ['#198754', '#fd7e14'],
            }
          ]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
        }
      }
    );
  }

  private destroyCharts(): void {

    this.attendanceTrendChart?.destroy();
    this.projectPieChart?.destroy();

    this.attendanceTrendChart = undefined;
    this.projectPieChart = undefined;
  }

  private formatDateLabel(dateValue: string): string {

    if (!dateValue)
    {
      return '';
    }

    const date = new Date(dateValue);

    if (Number.isNaN(date.getTime()))
    {
      return dateValue;
    }

    return date.toLocaleDateString(
      'en-GB',
      {
        day: '2-digit',
        month: 'short'
      }
    );
  }
}
