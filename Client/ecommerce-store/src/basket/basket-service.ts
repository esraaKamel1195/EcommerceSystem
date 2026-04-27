import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Router } from '@angular/router';
import { Basket, IBasketItem, IBasketTotal } from '../shared/models/basket';
import { IProduct } from '../shared/models/product';
import { AccountService } from '../account/account-service';

@Injectable({
  providedIn: 'root',
})
export class BasketService {
  private readonly base_url = 'https://id-local.eshopping.com:44344/basket/api/v1/BasketApi';
  // private readonly base_url = 'http://localhost:8010/Basket';

  private basketSource = new BehaviorSubject<Basket | null>(null);
  basketSource$ = this.basketSource.asObservable();

  private basketTotal = new BehaviorSubject<IBasketTotal | null>(null);
  basketTotal$ = this.basketTotal.asObservable();

  constructor(
    private http: HttpClient,
    private accountService: AccountService,
    private router: Router,
  ) {}

  getBasket(userName: string) {
    const httpOptions = {
      headers: {
        'Content-Type': 'application/json',
        Authorization: this.accountService.authorizationHeaderValue,
      },
    };

    return this.http.get<Basket>(`${this.base_url}/GetBasket/${userName}`, httpOptions).subscribe({
      next: (basket) => {
        this.basketSource.next(basket);
        this.calculateTotals();
      },
      error: (err) => console.log(err),
    });
  }

  setBasket(basket: Basket) {
    const httpOptions = {
      headers: {
        'Content-Type': 'application/json',
        Authorization: this.accountService.authorizationHeaderValue,
      },
    };

    return this.http.post<Basket>(`${this.base_url}/CreateBasket`, basket, httpOptions).subscribe({
      next: (response) => {
        this.basketSource.next(response);
        this.calculateTotals();
      },
      error: (err) => console.log(err),
    });
  }

  getCurrentBasketValue() {
    return this.basketSource.value;
  }

  addItemToBasket(item: IProduct, quantity: number = 1) {
    const itemToAdd: IBasketItem = this.mapProductItemToBasketItem(item);
    const basket = this.getCurrentBasketValue() ?? this.createBasket();

    basket.items = this.addOrUpdateItem(basket.items, itemToAdd, quantity);
    this.setBasket(basket);
  }

  mapProductItemToBasketItem(item: IProduct): IBasketItem {
    return {
      productId: item.id,
      productName: item.name,
      price: item.price,
      imageFile: item.imageFile,
      quantity: 0,
    };
  }

  createBasket(): Basket {
    const basket = new Basket();
    this.setBasket(basket);
    localStorage.setItem('basket_username', 'esraa');
    return basket;
  }

  addOrUpdateItem(items: IBasketItem[], itemToAdd: IBasketItem, quantity: number): IBasketItem[] {
    const existingItem = items.find((i) => i.productId === itemToAdd.productId);
    if (existingItem) {
      existingItem.quantity += quantity;
    } else {
      items.push({ ...itemToAdd, quantity });
    }
    return items;
  }

  incrementItemQuantity(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();
    if (!basket) return;
    const foundItemIndex = basket.items.findIndex((i) => i.productId === item.productId);
    if (foundItemIndex === -1) return;
    basket.items[foundItemIndex].quantity++;
    this.setBasket(basket);
  }

  decrementItemQuantity(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();
    if (!basket) return;
    const foundItemIndex = basket.items.findIndex((i) => i.productId === item.productId);
    if (foundItemIndex === -1) return;
    basket.items[foundItemIndex].quantity--;
    if (basket.items[foundItemIndex].quantity <= 0) {
      basket.items.splice(foundItemIndex, 1);
    }
    this.setBasket(basket);
  }

  removeItemFromBasket(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();
    if (!basket) return;
    //const foundItemIndex = basket.items.findIndex((i) => i.productId === item.productId);
    //if (foundItemIndex === -1) return;
    //basket.items.splice(foundItemIndex, 1);
    if (!basket.items.some((i) => i.productId === item.productId)) return;
    basket.items = basket.items.filter((i) => i.productId !== item.productId);
    if (basket.items.length === 0) {
      this.deleteBasket(basket);
      return;
    }
    this.setBasket(basket);
  }

  deleteBasket(basket: Basket) {
    const httpOptions = {
      headers: {
        'Content-Type': 'application/json',
        Authorization: this.accountService.authorizationHeaderValue,
      },
    };
    return this.http
      .delete(`${this.base_url}/DeleteBasket/${basket.userName}`, httpOptions)
      .subscribe({
        next: () => {
          this.basketSource.next(null);
          this.basketTotal.next(null);
        },
        error: (err) => console.log(err),
      });
  }

  checkoutBasket(basket: Basket) {
    const httpOptions = {
      headers: {
        'Content-Type': 'application/json',
        Authorization: this.accountService.authorizationHeaderValue,
      },
    };

    return this.http.post(`${this.base_url}/Checkout`, basket, httpOptions).subscribe({
      next: () => {
        this.basketSource.next(null);
        this.basketTotal.next(null);
        this.router.navigate(['/']);
        // this.deleteBasket(basket);
      },
      error: (err) => console.log(err),
    });
  }

  private calculateTotals() {
    const basket = this.getCurrentBasketValue();
    if (basket) {
      const total = basket.items.reduce((sum, item) => sum + item.price * item.quantity, 0);
      this.basketTotal.next({ total });
    }
  }
}
