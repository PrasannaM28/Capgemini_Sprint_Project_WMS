import { Component }
from '@angular/core';

import { Router }
from '@angular/router';

import { Auth }
from '../../services/auth';

@Component({
  selector: 'app-navbar',

  standalone: false,

  templateUrl: './navbar.html',

  styleUrl: './navbar.css'
})

export class Navbar {

  role = '';

  constructor(
    private authService:
      Auth,

    private router: Router
  )
  {
    this.role =
      this.authService.getRole();
  }

  logout(): void {

    this.authService.logout();

    this.router.navigate([
      '/login'
    ]);
  }
}
