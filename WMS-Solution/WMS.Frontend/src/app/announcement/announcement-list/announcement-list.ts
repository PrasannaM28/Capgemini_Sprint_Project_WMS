import {
  Component,
  OnInit
}
from '@angular/core';

import {
  FormBuilder,
  FormGroup,
  Validators
}
from '@angular/forms';

import { Auth }
from '../../services/auth';

import { Announcement }
from '../../services/announcement';

@Component({
  selector: 'app-announcement-list',
  standalone: false,
  templateUrl: './announcement-list.html',
  styleUrl: './announcement-list.css',
})

export class AnnouncementList
implements OnInit {

  announcements: any[] = [];

  announcementForm: FormGroup;

  role = '';

  editingAnnouncementId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: Auth,
    private announcementService:
      Announcement
  ) {
    this.announcementForm = this.fb.group({
      title: ['', Validators.required],
      message: ['', Validators.required],
    });
  }

  ngOnInit(): void {

    this.role = this.authService.getRole();

    this.loadAnnouncements();
  }

  get canManageAnnouncements(): boolean {

    return this.role === 'Admin';
  }

  get f() {

    return this.announcementForm.controls;
  }

  loadAnnouncements(): void {

    this.announcementService
      .getAll()
      .subscribe({

        next: (response) =>
        {
          this.announcements =
            response.data ?? [];
        }
      });
  }

  editAnnouncement(announcement: any): void {

    if (!this.canManageAnnouncements) {
      return;
    }

    this.editingAnnouncementId = announcement.announcementId;
    this.announcementForm.patchValue({
      title: announcement.title,
      message: announcement.message,
    });
  }

  cancelEdit(): void {

    this.editingAnnouncementId = null;
    this.announcementForm.reset();
  }

  saveAnnouncement(): void {

    if (!this.canManageAnnouncements) {
      return;
    }

    if (this.announcementForm.invalid) {
      this.announcementForm.markAllAsTouched();
      return;
    }

    const payload = {
      title: this.announcementForm.value.title,
      message: this.announcementForm.value.message,
      createdBy: 0,
    };

    const request = this.editingAnnouncementId
      ? this.announcementService.update(this.editingAnnouncementId, payload)
      : this.announcementService.create(payload);

    request.subscribe({
      next: () => {
        this.editingAnnouncementId = null;
        this.announcementForm.reset();
        this.loadAnnouncements();
      },
    });
  }

  deleteAnnouncement(id: number): void {

    if (!this.canManageAnnouncements) {
      return;
    }

    if (!confirm('Delete announcement?')) {
      return;
    }

    this.announcementService.delete(id).subscribe({
      next: () => this.loadAnnouncements(),
    });
  }
}