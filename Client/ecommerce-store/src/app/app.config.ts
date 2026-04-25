import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
// app.config.ts
import { provideAnimations } from '@angular/platform-browser/animations';
import { BsDropdownConfig } from 'ngx-bootstrap/dropdown';

import { routes } from './app.routes';
import { errorInterceptor } from '../core/interceptors/error-interceptor';
import { loadingInterceptor } from '../core/interceptors/loading-interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withInterceptors([errorInterceptor, loadingInterceptor])),
    provideAnimations(), // required for [isAnimated]="true"
    { provide: BsDropdownConfig, useValue: { isAnimated: true, autoClose: true } },
  ],
};
