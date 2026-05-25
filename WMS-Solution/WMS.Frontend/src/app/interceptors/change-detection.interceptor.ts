import { ApplicationRef, Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';

import { Observable } from 'rxjs';

@Injectable()
export class ChangeDetectionInterceptor implements HttpInterceptor {
  constructor(private appRef: ApplicationRef) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return new Observable<HttpEvent<any>>(observer => {
      const subscription = next.handle(request).subscribe({
        next: (event) => {
          observer.next(event);

          this.refreshViews();
        },
        error: (error) => {
          observer.error(error);

          this.refreshViews();
        },
        complete: () => {
          observer.complete();

          this.refreshViews();
        },
      });

      return () => subscription.unsubscribe();
    });
  }

  private refreshViews(): void {
    queueMicrotask(() => {
      this.appRef.components.forEach(componentRef => {
        componentRef.changeDetectorRef.detectChanges();
      });
    });
  }
}