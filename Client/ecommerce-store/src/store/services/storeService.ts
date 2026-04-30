import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IBrands, IProduct, ITypes } from '../../shared/models/product';
import { IResponseDto } from '../../shared/models/response';
import { StoreParams } from '../../shared/models/storeParams';
import { AccountService } from '../../account/account-service';

@Injectable({
  providedIn: 'root',
})
export class StoreService {
  private readonly base_url = 'https://id-local.eshopping.com:44344/catalog/api/v1/Catalog';
  // private readonly base_url = 'http://localhost:8010/Catalog';
  private readonly http = inject(HttpClient);
  private readonly accountService = inject(AccountService);

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

    const httpOptions = {
      headers: {
        'Content-Type': 'application/json',
        Authorization: this.accountService.authorizationHeaderValue,
      },
    };
    return this.http.get<IResponseDto<IProduct[]>>(`${this.base_url}/GetAllProducts`, {
      params,
      headers: httpOptions.headers,
    });
  }

  getAllBrands(): Observable<IBrands[]> {
    const httpOptions = {
      headers: {
        'Content-Type': 'application/json',
        Authorization: this.accountService.authorizationHeaderValue,
      },
    };
    return this.http.get<IBrands[]>(`${this.base_url}/GetAllBrands`, {
      headers: httpOptions.headers,
    });
  }

  getAllTypes(): Observable<ITypes[]> {
    const httpOptions = {
      headers: {
        'Content-Type': 'application/json',
        Authorization: this.accountService.authorizationHeaderValue,
      },
    };
    return this.http.get<ITypes[]>(`${this.base_url}/GetAllTypes`, {
      headers: httpOptions.headers,
    });
  }

  getProduct(id: string): Observable<IProduct> {
    const httpOptions = {
      headers: {
        'Content-Type': 'application/json',
        Authorization: this.accountService.authorizationHeaderValue,
      },
    };
    return this.http.get<IProduct>(`${this.base_url}/GetProductById/${id}`, {
      headers: httpOptions.headers,
    });
  }
}
