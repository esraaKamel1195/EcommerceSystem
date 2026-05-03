import { Component, effect, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Navbar } from '../core/components/navbar/navbar';
import { Header } from '../core/components/header/header';
// Import library module
import { NgxSpinnerModule } from 'ngx-spinner';
import { BasketService } from '../basket/basket-service';
import { AccountService } from '../account/account-service';
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Navbar, Header, NgxSpinnerModule],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit {
  protected readonly title = signal('ecommerce-store');
  private readonly basket = inject(BasketService);
  isUserAuthenticated: WritableSignal<boolean> = signal<boolean>(false);

  constructor(private accountService: AccountService) {
    effect(() => {
      console.log('from app', this.isUserAuthenticated());
      if (localStorage.getItem('token')) {
        this.isUserAuthenticated.set(true);
      } else {
        this.isUserAuthenticated.set(false);
      }
    });
  }

  ngOnInit(): void {
    const storedUserName = localStorage.getItem('basket_username');
    if (storedUserName) {
      this.basket.getBasket(storedUserName);
    }
  }
}
