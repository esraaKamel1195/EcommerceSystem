import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { BasketService } from '../../../basket/basket-service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-order-summary',
  imports: [CommonModule, RouterModule],
  templateUrl: './order-summary.html',
  styleUrl: './order-summary.scss',
})
export class OrderSummary {
  public basketService = inject(BasketService);
}
