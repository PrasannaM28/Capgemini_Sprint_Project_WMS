import { NgModule }
from '@angular/core';

import { CommonModule }
from '@angular/common';

import { ReactiveFormsModule }
from '@angular/forms';

import { SharedModule }
from '../shared/shared-module';

import { AnnouncementList }
from './announcement-list/announcement-list';

@NgModule({
  declarations: [AnnouncementList],
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  exports: [AnnouncementList]
})
export class AnnouncementModule {}
