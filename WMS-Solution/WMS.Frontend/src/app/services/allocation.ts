import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class Allocation {
  private apiUrl = `${environment.apiUrl}/allocation`;

  constructor(private http: HttpClient) {}

  allocate(data: any): Observable<any> {
    return this.http.post(this.apiUrl, data);
  }

  getAssignableEmployees(): Observable<any> {
    return this.http.get(`${this.apiUrl}/assignable-employees`);
  }

  getProjectAllocations(projectId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/project/${projectId}`);
  }

  deassign(projectId: number, empId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/project/${projectId}/employee/${empId}`);
  }
}