import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Loading } from '../services/loading';
import { delay, finalize } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingService = inject(Loading);
  loadingService.loading();

  return next(req).pipe(
    delay(1000),
    finalize(() => loadingService.idle())
  );
};
