import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { HttpClient } from '@angular/common/http';

import { EmployeeEdit } from './employee-edit';
import { Auth } from '../../services/auth';
import { Employee } from '../../services/employee';
import { Department } from '../../services/department';
import { UiFeedbackService } from '../../shared/ui-feedback/ui-feedback.service';

describe('EmployeeEdit', () => {
  let component: EmployeeEdit;
  let fixture: ComponentFixture<EmployeeEdit>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EmployeeEdit],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: () => null,
              },
            },
          },
        },
        {
          provide: Auth,
          useValue: {
            getRole: () => 'User',
          },
        },
        {
          provide: Employee,
          useValue: {
            getById: () =>
              of({
                data: {
                  employeeId: 0,
                  firstName: 'Test',
                  lastName: 'User',
                  phoneNumber: '1234567890',
                  genderId: 1,
                  departmentId: 1,
                  roleId: 1,
                },
              }),
          },
        },
        {
          provide: Department,
          useValue: {
            getAll: () => of({ data: [] }),
          },
        },
        {
          provide: HttpClient,
          useValue: {
            get: () => of({ data: [] }),
          },
        },
        {
          provide: UiFeedbackService,
          useValue: {
            success: () => undefined,
            error: () => undefined,
          },
        },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(EmployeeEdit);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
