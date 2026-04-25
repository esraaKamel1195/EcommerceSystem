import { Component, ElementRef, OnInit, signal, ViewChild, WritableSignal } from '@angular/core';
import { StoreService } from './services/storeService';
import { IBrands, IProduct, ITypes } from '../shared/models/product';
import { ProductItem } from './product-item/product-item';
import { StoreParams } from '../shared/models/storeParams';
import { PaginationModule, PageChangedEvent } from 'ngx-bootstrap/pagination';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-store',
  imports: [ProductItem, PaginationModule, FormsModule],
  templateUrl: './store.html',
  styleUrl: './store.scss',
})
export class Store implements OnInit {
  products: WritableSignal<IProduct[]> = signal([]);
  brands: IBrands[] = [];
  types: ITypes[] = [];
  storeParams: StoreParams = new StoreParams();
  sortOptions = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price :Ascending', value: 'priceAsc' },
    { name: 'Price :Descending', value: 'priceDesc' },
  ];

  itemsPerPageOptions = [
    { value: '10', name: 'Default' },
    { name: '5 / page', value: 5 },
    { name: '10 / page', value: 10 },
    { name: '20 / page', value: 20 },
    { name: '25 / page', value: 25 },
    { name: '50 / page', value: 50 },
  ];

  @ViewChild('searchTerm') searchTerm: ElementRef | undefined;
  totalItems = 0;
  currentPage = 1;
  itemsPerPage = 10;
  maxSize = 7; // max page buttons shown
  rotate = true; // keep current page centered
  boundaryLinks = true; // show First / Last buttons

  constructor(private store: StoreService) {}

  ngOnInit(): void {
    // Initialization logic can go here
    this.getAllProducts();

    this.getAallBrands();

    this.getAallTypes();
  }

  getAllProducts(): void {
    this.store.getAllProducts(this.storeParams).subscribe({
      next: (res) => {
        this.products.set(res.data);
        this.totalItems = res.count;
        this.currentPage = res.pageIndex;
        this.itemsPerPage = res.pageSize;
        this.storeParams.pageNumber = res.pageIndex;
        this.storeParams.pageSize = res.pageSize;

        console.log('products', this.products());
      },
      error: (err) => {
        console.error('Error fetching products', err);
      },
    });
  }

  getAallBrands(): void {
    this.store.getAallBrands().subscribe({
      next: (res) => {
        console.log('brands', res);
        this.brands = [{ id: '', name: 'All Brands' }, ...res];
      },
      error: (err) => {
        console.error('Error fetching brands', err);
      },
    });
  }

  getAallTypes(): void {
    this.store.getAallTypes().subscribe({
      next: (res) => {
        console.log('types', res);
        this.types = [{ id: '', name: 'All Types' }, ...res];
      },
      error: (err) => {
        console.error('Error fetching types', err);
      },
    });
  }

  onTypeSelected(typeId: string) {
    this.storeParams.typeId = typeId;
    this.getAllProducts();
  }

  onBrandSelected(brandId: string) {
    this.storeParams.brandId = brandId;
    this.getAllProducts();
  }

  onSortSelected(sort: any) {
    this.storeParams.sort = sort.value;
    this.getAllProducts();
  }

  onPageChanged(event: PageChangedEvent) {
    this.storeParams.pageNumber = event.page;
    this.getAllProducts();
  }

  onSearch() {
    console.log('searchTerm', this.searchTerm?.nativeElement.value);
    this.storeParams.search = this.searchTerm?.nativeElement.value;
    this.storeParams.pageNumber = 1; // Reset to first page on new search
    this.getAllProducts();
  }

  onResetFilters() {
    this.storeParams.brandId = '';
    this.storeParams.typeId = '';
    this.getAllProducts();
  }

  onItemsPerPageChanged(event: any) {
    this.storeParams.pageSize = event.value;
    this.storeParams.pageNumber = 1; // Reset to first page when page size changes
    this.getAllProducts();
  }
}
