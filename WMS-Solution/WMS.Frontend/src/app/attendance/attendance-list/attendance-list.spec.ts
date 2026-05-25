import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';

import { AttendanceList } from './attendance-list';

describe('AttendanceList', () => {
  let component: AttendanceList;
  let fixture: ComponentFixture<AttendanceList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AttendanceList],
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
