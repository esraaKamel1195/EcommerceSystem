import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError((error) => {
      console.error('HTTP Error:', error);
      if (error.status === 400) {
        // Handle Bad Request errors
        console.error('Bad Request:', error.error);
        router.navigate(['/server-error']);
      } else if (error.status === 404) {
        // Handle Not Found errors
        console.error('Not Found:', error.error);
        router.navigate(['/not-found']);
      } else if (error.status === 401) {
        // Handle Unauthorized errors
        console.error('Unauthorized:', error.error);
        router.navigate(['/unauthorized']);
      } else if (error.status === 500) {
        // Handle Internal Server errors
        console.error('Internal Server Error:', error.error);
        router.navigate(['/server-error']);
      } else {
        // Handle other types of errors
        console.error('An unexpected error occurred:', error.error);
        router.navigate(['/server-error']);
      }
      return throwError(() => new Error(error));
    }),
  );
};
