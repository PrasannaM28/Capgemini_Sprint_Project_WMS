import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { of } from 'rxjs';

import { AnnouncementList } from './announcement-list';
import { Auth } from '../../services/auth';
import { Announcement } from '../../services/announcement';

describe('AnnouncementList', () => {
  let component: AnnouncementList;
  let fixture: ComponentFixture<AnnouncementList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AnnouncementList],
      providers: [
        {
          provide: Auth,
          useValue: {
            getRole: () => 'User',
          },
        },
        {
          provide: Announcement,
          useValue: {
            getAll: () => of({ data: [] }),
          },
        },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(AnnouncementList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
