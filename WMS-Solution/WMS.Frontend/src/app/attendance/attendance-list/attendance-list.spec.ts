import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { of } from 'rxjs';

import { AttendanceList } from './attendance-list';
import { Auth } from '../../services/auth';
import { Attendance } from '../../services/attendance';
import { Employee } from '../../services/employee';

describe('AttendanceList', () => {
  let component: AttendanceList;
  let fixture: ComponentFixture<AttendanceList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AttendanceList],
      providers: [
        {
          provide: Auth,
          useValue: {
            getRole: () => 'User',
            getUsername: () => '',
          },
        },
        {
          provide: Attendance,
          useValue: {
            getMonthlyAttendance: () => of({ data: [] }),
          },
        },
        {
          provide: Employee,
          useValue: {
            getAll: () => of({ data: [] }),
            search: () => of({ data: [] }),
          },
        },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(AttendanceList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
