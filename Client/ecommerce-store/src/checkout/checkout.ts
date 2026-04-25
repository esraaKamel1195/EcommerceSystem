import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IBasket, IBasketItem } from '../shared/models/basket';
import { BasketService } from '../basket/basket-service';
import { AccountService } from '../account/account-service';

@Component({
  selector: 'app-checkout',
  imports: [CommonModule],
  templateUrl: './checkout.html',
  styleUrl: './checkout.scss',
})
export class Checkout implements OnInit {
  isUserAuthenticated: boolean = false;

  constructor(
    public basketService: BasketService,
    private accountService: AccountService,
  ) {}

  ngOnInit(): void {
    this.accountService.currentUser$.subscribe({
      next: (isAuthenticated) => {
        this.isUserAuthenticated = isAuthenticated;
      },
      error: (err) => console.log(err),
    });
  }

  removeBasketItem(item: IBasketItem) {
    this.basketService.removeItemFromBasket(item);
  }

  incrementItem(item: IBasketItem) {
    this.basketService.incrementItemQuantity(item);
  }

  decrementItem(item: IBasketItem) {
    this.basketService.decrementItemQuantity(item);
  }

  orderNow(item: IBasket) {
    this.basketService.checkoutBasket(item);
  }
}
