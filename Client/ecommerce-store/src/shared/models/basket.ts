export interface IBasketItem {
  quantity: number;
  imageFile: string;
  price: number;
  priceAfterDiscount: number;
  productId: string;
  productName: string;
}

export interface IBasket {
  userName: string;
  items: IBasketItem[];
  totalPrice: number;
}

export class Basket implements IBasket {
  userName: string = 'esraa';
  totalPrice: number = 0;
  items: IBasketItem[] = [];
}

export interface IBasketTotal {
  total: number;
}
