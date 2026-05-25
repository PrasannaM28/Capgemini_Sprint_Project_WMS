import { Injectable }
from '@angular/core';

import { HttpClient }
from '@angular/common/http';

import {
  BehaviorSubject,
  Observable
}
from 'rxjs';

import { tap }
from 'rxjs/operators';

import { environment }
from '../../environments/environment';

import { jwtDecode }
from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})

export class Auth {

  private apiUrl =
    `${environment.apiUrl}/auth`;

  private loggedIn =
    new BehaviorSubject<boolean>(
      this.hasToken()
    );

  loggedIn$ =
    this.loggedIn.asObservable();

  constructor(
    private http: HttpClient
  ) { }

  login(data: any):
    Observable<any>
  {
    return this.http.post(
      `${this.apiUrl}/login`,
      data
    )

    .pipe(
      tap((response: any) =>
      {
        localStorage.setItem(
          'token',
          response.data.token
        );

        this.loggedIn.next(true);
      })
    );
  }

  changePassword(data: any):
    Observable<any>
  {
    return this.http.post(
      `${this.apiUrl}/change-password`,
      data
    );
  }

  logout(): void {

    localStorage.removeItem(
      'token'
    );

    this.loggedIn.next(false);
  }

  getToken():
    string | null
  {
    return localStorage.getItem(
      'token'
    );
  }

  hasToken(): boolean {

    return !!localStorage.getItem(
      'token'
    );
  }

  getRole(): string {

    const token =
      this.getToken();

    if (!token)
    {
      return '';
    }

    const decoded: any =
      jwtDecode(token);

    return decoded[
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
    ] ?? '';
  }

  getUsername(): string {

    const token =
      this.getToken();

    if (!token)
    {
      return '';
    }

    const decoded: any =
      jwtDecode(token);

    return String(
      decoded.unique_name
      ?? decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
      ?? ''
    );
  }

  getUserId(): number {

    const token =
      this.getToken();

    if (!token)
    {
      return 0;
    }

    const decoded: any =
      jwtDecode(token);

    return Number(
      decoded.sub
      ?? decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']
      ?? 0
    );
  }
}