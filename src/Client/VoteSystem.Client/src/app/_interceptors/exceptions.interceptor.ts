import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { catchError, throwError } from 'rxjs';
import { AppError } from '../_models/_contracts/appError';

export const exceptionsInterceptor: HttpInterceptorFn = (req, next) => {
  const toastr = inject(ToastrService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const errorArray: AppError[] = error.error;

      switch (error.status) {
        case 400:
          console.log('400');
          errorArray
            ? errorArray.map((e) => {
                toastr.error(e.description, e.code);
              })
            : toastr.error('Bad request');
          break;
        case 401:
          console.log('401');
          errorArray
            ? errorArray.map((e) => {
                toastr.error(e.description, e.code);
              })
            : toastr.error('Unauthorized');
          break;
        case 404:
          console.log('404');
          toastr.error('Not found');
          break;
        case 500:
          console.log('500');
          toastr.error('Interval server error');
          break;
        default:
          toastr.error('Unexpected error occurs');
      }

      return throwError(() => new Error(error.message));
    })
  );
};
