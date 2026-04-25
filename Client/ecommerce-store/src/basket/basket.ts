import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { BasketService } from './basket-service';
import { OrderSummary } from '../shared/components/order-summary/order-summary';

@Component({
  selector: 'app-basket',
  imports: [CommonModule, OrderSummary],
  templateUrl: './basket.html',
  styleUrl: './basket.scss',
})
export class Basket {
  constructor(public basketService: BasketService) {}

  decrease(item: any) {
    this.basketService.decrementItemQuantity(item);
  }

  increase(item: any) {
    this.basketService.incrementItemQuantity(item);
  }

  remove(item: any) {
    this.basketService.removeItemFromBasket(item);
  }
}
