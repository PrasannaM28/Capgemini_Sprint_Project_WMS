import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LeaveApply } from './leave-apply';

describe('LeaveApply', () => {
  let component: LeaveApply;
  let fixture: ComponentFixture<LeaveApply>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LeaveApply],
    }).compileComponents();

    fixture = TestBed.createComponent(LeaveApply);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
