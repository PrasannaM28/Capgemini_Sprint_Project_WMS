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

export class Leave {

  private apiUrl =
    `${environment.apiUrl}/leave`;

  constructor(
    private http: HttpClient
  ) { }

  apply(data: any):
    Observable<any>
  {
    return this.http.post(
      this.apiUrl,
      data
    );
  }

  getAll(): Observable<any> {
    return this.http.get(this.apiUrl);
  }

  approve(data: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/approve`, data);
  }

  cancel(leaveId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/cancel/${leaveId}`, {});
  }
}