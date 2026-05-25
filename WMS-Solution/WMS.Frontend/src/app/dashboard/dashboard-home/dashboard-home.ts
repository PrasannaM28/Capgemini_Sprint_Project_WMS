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

  private chart?: Chart;

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

    this.chart?.destroy();

    this.routerSubscription?.unsubscribe();
  }

  loadDashboard(): void {

    this.dashboardService
      .getDashboard()
      .subscribe({

        next: (response) =>
        {
          this.chart?.destroy();

          this.dashboardData =
            response.data;

          this.loadChart();
        }
      });
  }

  get summaryCards(): Array<{ label: string; value: number }> {

    const data = this.dashboardData ?? {};

    if (this.role === 'Admin') {
      return [
        { label: 'Employees', value: data.totalEmployees ?? 0 },
        { label: 'Departments', value: data.totalDepartments ?? 0 },
        { label: 'Projects', value: data.totalProjects ?? 0 },
        { label: 'Pending Leaves', value: data.pendingLeaves ?? 0 },
        { label: 'Announcements', value: data.activeAnnouncements ?? 0 },
        { label: 'Hours', value: data.totalHours ?? 0 },
      ];
    }

    if (this.role === 'Manager') {
      return [
        { label: 'Team Members', value: data.totalEmployees ?? 0 },
        { label: 'Projects', value: data.totalProjects ?? 0 },
        { label: 'Pending Leaves', value: data.pendingLeaves ?? 0 },
        { label: 'Announcements', value: data.activeAnnouncements ?? 0 },
        { label: 'Hours', value: data.totalHours ?? 0 },
      ];
    }

    return [
      { label: 'Projects', value: data.totalProjects ?? 0 },
      { label: 'Pending Leaves', value: data.pendingLeaves ?? 0 },
      { label: 'Announcements', value: data.activeAnnouncements ?? 0 },
      { label: 'Hours', value: data.totalHours ?? 0 },
    ];
  }

  loadChart(): void {

    this.chart = new Chart(
      'dashboardChart',
      {
        type: 'bar',

        data: {

          labels: [
            ...this.summaryCards.map(card => card.label)
          ],

          datasets: [
            {
              label:
                'WMS Dashboard',

              data: [
                ...this.summaryCards.map(card => card.value)
              ]
            }
          ]
        }
      }
    );
  }

  formatHours(hours: number): string {

    const totalSeconds =
      Math.max(
        0,
        Math.round(hours * 3600)
      );

    const hoursPart =
      Math.floor(totalSeconds / 3600);

    const minutesPart =
      Math.floor((totalSeconds % 3600) / 60);

    const secondsPart =
      totalSeconds % 60;

    return [
      hoursPart,
      minutesPart,
      secondsPart
    ]
      .map(part =>
        String(part).padStart(2, '0'))
      .join(':');
  }
}
