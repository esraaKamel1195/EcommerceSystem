import { Routes } from '@angular/router';
import { Home } from '../home/home';
import { NotFound } from '../core/pages/not-found/not-found';
import { NotAuthentication } from '../core/pages/not-authentication/not-authentication';
import { ServerError } from '../core/pages/server-error/server-error';
import { SignoutRedirectCallbackComponent } from '../account/signout-redirect-callback/signout-redirect-callback.component';
import { SigninRedirectCallbackComponent } from '../account/signin-redirect-callback/signin-redirect-callback.component';
import { AuthGuard } from '../core/guards/auth.guard';

export const routes: Routes = [
  // { path: '', component: Home, pathMatch: 'full', data: { breadcrumb: 'Home' } },
  { path: '', redirectTo: '/store', pathMatch: 'full', data: { breadcrumb: 'Store' } },
  {
    path: 'store',
    loadChildren: () => import('../store/store.route').then((m) => m.storeRoutes),
    data: { breadcrumb: 'Store' },
  },
  {
    path: 'basket',
    loadChildren: () => import('../basket/basket.route').then((m) => m.basketRoutes),
    data: { breadcrumb: 'Basket' },
  },
  {
    path: 'account',
    loadChildren: () => import('../account/account.route').then((m) => m.accountRoutes),
    data: { breadcrumb: { skip: true } },
  },
  {
    path: 'checkout',
    canActivate: [AuthGuard],
    loadChildren: () => import('../checkout/checkout.route').then((m) => m.checkoutRoutes),
    data: { breadcrumb: 'Checkout' },
  },
  {
    path: 'signin-callback',
    component: SigninRedirectCallbackComponent,
    pathMatch: 'full'
  },
  {
    path: 'signout-callback',
    redirectTo: '/account/login',

  },
  { path: 'not-found', component: NotFound },
  { path: 'unauthorized', component: NotAuthentication },
  { path: 'server-error', component: ServerError },
  { path: '**', redirectTo: 'store' },
];
