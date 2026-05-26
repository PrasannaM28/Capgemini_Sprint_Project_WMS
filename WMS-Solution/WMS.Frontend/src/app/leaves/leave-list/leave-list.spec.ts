import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { of } from 'rxjs';

import { LeaveList } from './leave-list';
import { Auth } from '../../services/auth';
import { Employee } from '../../services/employee';
import { Leave } from '../../services/leave';

describe('LeaveList', () => {
  let component: LeaveList;
  let fixture: ComponentFixture<LeaveList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LeaveList],
      providers: [
        {
          provide: Auth,
          useValue: {
            getRole: () => 'User',
            getUsername: () => '',
            getUserId: () => 1,
          },
        },
        {
          provide: Leave,
          useValue: {
            getAll: () => of({ data: [] }),
          },
        },
        {
          provide: Employee,
          useValue: {
            search: () => of({ data: [] }),
          },
        },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(LeaveList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
