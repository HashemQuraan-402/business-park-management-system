import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const unauthorizedInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((err) => {
      if (err.status === 401 && auth.isLoggedIn()) {
        // Token expired or was rejected - clear the stale session and send back to login.
        auth.logout();
        router.navigate(['/login']);
      }
      return throwError(() => err);
    })
  );
};
