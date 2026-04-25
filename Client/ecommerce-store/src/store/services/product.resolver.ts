import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { StoreService } from './storeService';

export const productResolver: ResolveFn<{ name: string }> =
  (route) => inject(StoreService).getProduct(route.paramMap.get('id')!);
