import { Injectable } from '@angular/core';

import { HttpClient }
from '@angular/common/http';

import { Observable }
from 'rxjs';

import { environment }
from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})

export class Attendance {

  private apiUrl =
    `${environment.apiUrl}/attendance`;

  constructor(
    private http: HttpClient
  ) { }

  checkIn(data: any):
    Observable<any>
  {
    return this.http.post(
      `${this.apiUrl}/checkin`,
      data
    );
  }

  checkOut(data: any):
    Observable<any>
  {
    return this.http.put(
      `${this.apiUrl}/checkout`,
      data
    );
  }

  getMonthlyAttendance(
    employeeId: number,
    month: number,
    year: number
  ): Observable<any>
  {
    return this.http.get(

      `${this.apiUrl}/monthly?employeeId=${employeeId}&month=${month}&year=${year}`
    );
  }
}
