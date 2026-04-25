import { Component, input, InputSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from "@angular/router";
import { BasketService } from '../../basket/basket-service';
import { IProduct } from '../../shared/models/product';

@Component({
  selector: 'app-product-item',
  imports: [CommonModule, RouterLink],
  templateUrl: './product-item.html',
  styleUrl: './product-item.scss',
})
export class ProductItem {
  product: InputSignal<IProduct | null> = input<IProduct | null>(null);

  constructor(private basketService: BasketService) {}

  addItemToBasket() {
    const product = this.product();
    if (product) {
      this.basketService.addItemToBasket(product, 1);
    }
  }
}
