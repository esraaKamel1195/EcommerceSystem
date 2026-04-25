import { Routes } from '@angular/router';
import { Basket } from './basket';

export const basketRoutes: Routes = [
  { path: '', component: Basket, data: { breadcrumb: 'Basket' } },
]
