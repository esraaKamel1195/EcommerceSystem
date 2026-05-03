import { Routes } from '@angular/router';

export const checkoutRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./checkout').then((m) => m.Checkout),
    data: { breadcrumb: 'Checkout' },
  },
];
