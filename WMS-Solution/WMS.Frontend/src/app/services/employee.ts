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
export class Employee {

  private apiUrl =
    `${environment.apiUrl}/employee`;

  constructor(
    private http: HttpClient
  ) { }

  getAll():
    Observable<any>
  {
    return this.http.get(
      this.apiUrl
    );
  }

  create(data: any):
    Observable<any>
  {
    return this.http.post(
      this.apiUrl,
      data
    );
  }

  update(data: any):
    Observable<any>
  {
    return this.http.put(
      `${this.apiUrl}/${data.employeeId}`,
      data
    );
  }

  delete(id: number):
    Observable<any>
  {
    return this.http.delete(
      `${this.apiUrl}/${id}`
    );
  }

  getById(id: number):
    Observable<any>
  {
    return this.http.get(
      `${this.apiUrl}/${id}`
    );
  }

  getPaged(
    pageNumber: number,
    pageSize: number
  ): Observable<any>
  {
    return this.http.get(

      `${this.apiUrl}/paged?pageNumber=${pageNumber}&pageSize=${pageSize}`
    );
  }

  search(
    searchTerm: string
  ): Observable<any>
  {
    return this.http.get(

      `${this.apiUrl}/search?searchTerm=${searchTerm}`
    );
  }
}
