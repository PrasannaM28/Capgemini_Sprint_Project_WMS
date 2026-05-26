import { Component, signal } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('wms-frontend');

  protected readonly showShell = signal(false);

  constructor(private router: Router) {
    this.updateShellVisibility(this.router.url);

    this.router.events
      .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe((event) => {
        this.updateShellVisibility(event.urlAfterRedirects);
      });
  }

  private updateShellVisibility(url: string): void {
    this.showShell.set(!url.startsWith('/login'));
  }
}
