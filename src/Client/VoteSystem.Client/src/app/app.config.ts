import {
  ApplicationConfig,
  importProvidersFrom,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { NgxSpinnerModule } from 'ngx-spinner';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { loadingInterceptor } from './_interceptors/loading.interceptor';
import { httpInterceptor } from './_interceptors/http.interceptor';
import { provideToastr } from 'ngx-toastr';
import { exceptionsInterceptor } from './_interceptors/exceptions.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(
      withInterceptors([
        loadingInterceptor,
        httpInterceptor,
        exceptionsInterceptor,
      ])
    ),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    importProvidersFrom(NgxSpinnerModule.forRoot({ type: 'ball-fall' })),
    provideAnimations(),
    provideToastr({ positionClass: 'toast-bottom-right' }),
  ],
};
