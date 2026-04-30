export interface IProduct {
  id: string;
  name: string;
  summary: string;
  description: string;
  imageFile: string;
  brands: IBrands;
  types: ITypes;
  price: number;
  priceAfterDiscount: number;
  hasDiscount: boolean;
  discountAmount: number;
  [key: string]: any;  // Add this line
}

export interface IBrands {
  id: string;
  name: string;
}

export interface ITypes {
  id: string;
  name: string;
}
