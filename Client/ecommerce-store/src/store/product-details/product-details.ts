import { Component, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { BreadcrumbService } from 'xng-breadcrumb';
import { IProduct } from '../../shared/models/product';
import { StoreService } from '../services/storeService';
import { BasketService } from '../../basket/basket-service';

@Component({
  selector: 'app-product-details',
  imports: [FormsModule, CommonModule],
  providers: [BreadcrumbService],
  templateUrl: './product-details.html',
  styleUrl: './product-details.scss',
})
export class ProductDetails implements OnInit {
  product: WritableSignal<IProduct | null> = signal(null);
  quantity = 1;
  breadcrumbService = inject(BreadcrumbService);

  constructor(
    private store: StoreService,
    private activatedRoute: ActivatedRoute,
    private basketService: BasketService
  ) {}

  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct() {
    this.activatedRoute.params.subscribe((params) => {
      const id = params['id'];
      this.store.getProduct(id).subscribe((product) => {
        // Do something with the product details
        console.log('Product details:', product);
        this.product?.set(product);
      });
    });
  }

  dec() {
    if (this.quantity > 1) this.quantity--;
  }

  inc() {
    this.quantity++;
  }

  addToCart() {
    // purely demo; no API
    const product = this.product();
    if (product) {
      this.basketService.addItemToBasket(product, this.quantity);
    }
  }
}
