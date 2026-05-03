import { CommonModule } from '@angular/common';
import { Component, effect, signal, WritableSignal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { BasketService } from '../../../basket/basket-service';
import { IBasketItem } from '../../../shared/models/basket';
import { AccountService } from '../../../account/account-service';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, RouterModule, CollapseModule, BsDropdownModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
})
export class Navbar {
  isNavbarCollapsed = true;
  isUserAuthenticated: WritableSignal<boolean> = signal<boolean>(false);

  constructor(
    public basketService: BasketService,
    private accountService: AccountService,
  ) {
    effect(() => {
      if (localStorage.getItem('token')) {
        this.isUserAuthenticated.set(true);
      } else {
        this.isUserAuthenticated.set(false);
      }
      console.log('Effect triggered, isUserAuthenticated:', this.isUserAuthenticated());
    });
  }

  ngOnInit(): void {
    this.accountService.currentUser$.subscribe({
      next: (res) => {
        this.isUserAuthenticated.set(res);
        console.log('is authenticated', this.isUserAuthenticated());
      },
      error: (err) => {
        console.log('An error occured while setting authenticated flag');
      },
    });
  }

  getBasketItemsCount(items: IBasketItem[]): number {
    return items.reduce((count, item) => count + item.quantity, 0);
  }

  login() {
    this.accountService.login();
  }

  logout() {
    this.accountService.logout();
    this.accountService.finishLogout();
  }
}
