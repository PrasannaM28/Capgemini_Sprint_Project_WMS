import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { of } from 'rxjs';

import { EmployeeList } from './employee-list';
import { Auth } from '../../services/auth';
import { Employee } from '../../services/employee';

describe('EmployeeList', () => {
  let component: EmployeeList;
  let fixture: ComponentFixture<EmployeeList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EmployeeList],
      providers: [
        {
          provide: Auth,
          useValue: {
            getRole: () => 'User',
            getUsername: () => '',
          },
        },
        {
          provide: Employee,
          useValue: {
            getPaged: () =>
              of({
                data: {
                  items: [],
                  totalRecords: 0,
                },
              }),
            search: () => of({ data: [] }),
          },
        },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(EmployeeList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
