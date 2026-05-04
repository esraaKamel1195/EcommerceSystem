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
  checkAuthenticated: WritableSignal<string | undefined> = signal<string | undefined>(undefined);
  isUserAuthenticated: WritableSignal<boolean> = signal<boolean>(false);

  constructor(private accountService: AccountService) {
    effect(() => {
      console.log('from app', this.isUserAuthenticated());
      // if (this.checkAuthenticated()) {
      //   this.isUserAuthenticated.set(true);
      // } else {
      //   this.isUserAuthenticated.set(false);
      // }

      this.isUserAuthenticated();
    });
  }

  ngOnInit(): void {
    // this.checkAuthenticated.set(localStorage.getItem('token') ?? undefined);

    this.accountService.currentUser$.subscribe({
      next: (res) => {
        this.isUserAuthenticated.set(res);
        console.log('is authenticated', this.isUserAuthenticated());
      },
      error: (err) => {
        console.log('An error occured while setting authenticated flag');
      },
    });

    const storedUserName = localStorage.getItem('basket_username');
    if (storedUserName) {
      this.basket.getBasket(storedUserName);
    }
  }
}
