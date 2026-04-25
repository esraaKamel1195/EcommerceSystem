import { Routes } from '@angular/router';
import { productResolver } from './services/product.resolver';

export const storeRoutes: Routes = [
  { path: '', loadComponent: () => import('./store').then((m) => m.Store) },
  {
    path: ':id',
    loadComponent: () => import('./product-details/product-details').then((m) => m.ProductDetails),
    resolve: {
      product: productResolver,
    },
    data: { breadcrumb: (data: any) => data['product']?.name ?? 'product' },
  },
];
