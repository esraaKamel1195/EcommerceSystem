import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IBrands, IProduct, ITypes } from '../../shared/models/product';
import { IResponseDto } from '../../shared/models/response';
import { StoreParams } from '../../shared/models/storeParams';

@Injectable({
  providedIn: 'root',
})
export class StoreService {
  private readonly base_url = 'http://localhost:8010/Catalog';
  private readonly http = inject(HttpClient);

  getAllProducts(storeParams: StoreParams): Observable<IResponseDto<IProduct[]>> {
    let params = new HttpParams();
    if (storeParams.search) {
      params = params.append('Search', storeParams.search);
    }
    if (storeParams.brandId) {
      params = params.append('BrandId', storeParams.brandId);
    }
    if (storeParams.typeId) {
      params = params.append('TypeId', storeParams.typeId);
    }
    if (storeParams.sort) {
      params = params.append('Sort', storeParams.sort);
    }
    params = params.append('PageIndex', storeParams.pageNumber.toString());
    params = params.append('PageSize', storeParams.pageSize.toString());

    return this.http.get<IResponseDto<IProduct[]>>(`${this.base_url}/GetAllProducts`, { params });
  }

  getAallBrands(): Observable<IBrands[]> {
    return this.http.get<IBrands[]>(`${this.base_url}/GetAllBrands`);
  }

  getAallTypes(): Observable<ITypes[]> {
    return this.http.get<ITypes[]>(`${this.base_url}/GetAllTypes`);
  }

  getProduct(id: string): Observable<IProduct> {
    return this.http.get<IProduct>(`${this.base_url}/GetProductById/${id}`);
  }
}
