import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { Router } from '@angular/router';
import { EMPTY, of } from 'rxjs';

import { DashboardHome } from './dashboard-home';
import { Auth } from '../../services/auth';
import { Dashboard } from '../../services/dashboard';

describe('DashboardHome', () => {
  let component: DashboardHome;
  let fixture: ComponentFixture<DashboardHome>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DashboardHome],
      providers: [
        {
          provide: Auth,
          useValue: {
            getRole: () => 'User',
          },
        },
        {
          provide: Dashboard,
          useValue: {
            getDashboard: () => of({ data: {} }),
          },
        },
        {
          provide: Router,
          useValue: {
            events: EMPTY,
            navigate: () => Promise.resolve(true),
          },
        },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardHome);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
