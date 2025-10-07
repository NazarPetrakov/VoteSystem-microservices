import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { LoadingService } from '../_services/loading.service';
import { delay, finalize } from 'rxjs';
import { SKIP_LOADING } from './loading-context';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingService = inject(LoadingService);

  const skip = req.context.get(SKIP_LOADING);
  if (skip) return next(req);

  loadingService.loadingOn();
  return next(req).pipe(
    delay(1000),
    finalize(() => loadingService.loadingOff())
  );
};
