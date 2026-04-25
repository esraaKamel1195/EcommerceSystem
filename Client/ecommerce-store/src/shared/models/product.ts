export interface IProduct {
  id: string;
  name: string;
  summary: string;
  description: string;
  imageFile: string;
  brands: IBrands;
  types: ITypes;
  price: number;
}

export interface IBrands {
  name: string;
  id: string;
}

export interface ITypes {
  name: string;
  id: string;
}
