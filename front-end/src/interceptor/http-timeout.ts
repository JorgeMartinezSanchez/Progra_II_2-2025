import { HttpInterceptorFn } from '@angular/common/http';
import { timeout, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

export const httpTimeoutInterceptor: HttpInterceptorFn = (req, next) => {
  const timeoutValue = 15000; // 15 segundos

  return next(req).pipe(
    timeout(timeoutValue),
    catchError(error => {
      console.error('⏰ Request timed out:', req.url);
      return throwError(() => new Error('Request timeout - Server not responding'));
    })
  );
};