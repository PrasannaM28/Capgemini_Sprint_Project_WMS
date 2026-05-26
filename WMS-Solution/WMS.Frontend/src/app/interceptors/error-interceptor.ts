import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest
}
from '@angular/common/http';

import { Injectable }
from '@angular/core';

import { UiFeedbackService } from '../shared/ui-feedback/ui-feedback.service';

import {
  Observable,
  throwError
}
from 'rxjs';

import { catchError }
from 'rxjs/operators';

@Injectable()

export class ErrorInterceptor
implements HttpInterceptor {

  constructor(private feedback: UiFeedbackService) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {

    return next.handle(request)

      .pipe(

        catchError(
          (
            error:
            HttpErrorResponse
          ) =>
          {
            let message =
              'Something went wrong';

            if (
              error.error?.message
            )
            {
              message =
                error.error.message;
            }

            this.feedback.error('Request failed', message);

            return throwError(
              () => error
            );
          }
        )
      );
  }
}
