import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpErrorResponse,
  HTTP_INTERCEPTORS,
} from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(
    req: import('@angular/common/http').HttpRequest<any>,
    next: import('@angular/common/http').HttpHandler
  ): import('rxjs').Observable<import('@angular/common/http').HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error) => {
        if (
          error.status === 401 ||
          error.status === 403 ||
          error.status === 404
        ) {
          return throwError(error.statusText);
        }
        if (error.status === 500) {
          return throwError(error.error);
        }
        if (error instanceof HttpErrorResponse) {
          const aplicationError = error.headers.get('Aplication-Error');
          if (aplicationError) {
            return throwError(aplicationError);
          }
          const serverError = error.error.errors;
          let modalStateErrors = '';
          if (serverError && typeof serverError === 'object') {
            for (const key in serverError) {
              if (serverError[key]) {
                modalStateErrors = modalStateErrors + serverError[key];
              }
            }
          }
          return throwError(modalStateErrors || serverError || 'Server Error');
        }
      })
    );
  }
}

export const ErrorInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: ErrorInterceptor,
  multi: true,
};
