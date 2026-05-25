import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';

import { LeaveList } from './leave-list';

describe('LeaveList', () => {
  let component: LeaveList;
  let fixture: ComponentFixture<LeaveList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LeaveList],
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
