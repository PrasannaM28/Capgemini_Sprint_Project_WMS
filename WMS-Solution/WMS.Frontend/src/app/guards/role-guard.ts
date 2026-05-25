import { Injectable }
from '@angular/core';

import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router
}
from '@angular/router';

import { Auth }
from '../services/auth';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard
implements CanActivate {

  constructor(
    private authService: Auth,
    private router: Router
  ) { }

  canActivate(
    route: ActivatedRouteSnapshot
  ): boolean {

    const allowedRoles =
      route.data['roles'] as string[] | undefined;

    if (!allowedRoles || allowedRoles.length === 0)
    {
      return true;
    }

    const currentRole =
      this.authService.getRole();

    if (allowedRoles.includes(currentRole))
    {
      return true;
    }

    this.router.navigate([
      '/dashboard'
    ]);

    return false;
  }
}
