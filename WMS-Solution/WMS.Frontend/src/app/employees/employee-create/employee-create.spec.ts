import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { of } from 'rxjs';
import { HttpClient } from '@angular/common/http';

import { EmployeeCreate } from './employee-create';
import { Department } from '../../services/department';
import { Employee } from '../../services/employee';
import { UiFeedbackService } from '../../shared/ui-feedback/ui-feedback.service';

describe('EmployeeCreate', () => {
  let component: EmployeeCreate;
  let fixture: ComponentFixture<EmployeeCreate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EmployeeCreate],
      providers: [
        {
          provide: Department,
          useValue: {
            getAll: () => of({ data: [] }),
          },
        },
        {
          provide: Employee,
          useValue: {
            create: () => of({}),
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

    fixture = TestBed.createComponent(EmployeeCreate);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
