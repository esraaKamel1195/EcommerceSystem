import {
  Component,
  computed,
  ElementRef,
  OnInit,
  Signal,
  signal,
  ViewChild,
  WritableSignal,
} from '@angular/core';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { StoreService } from './services/storeService';
import { IBrands, IProduct, ITypes } from '../shared/models/product';
import { ProductItem } from './product-item/product-item';
import { StoreParams } from '../shared/models/storeParams';
import { PaginationModule, PageChangedEvent } from 'ngx-bootstrap/pagination';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ColumnsConfig, DataType } from '../shared/models/ColumnsConfig';

interface ContextMenuItemsMenu {
  label?: string;
  icon?: string;
  action?: () => void;
  items?: ContextMenuItemsMenu[];
  separator?: boolean;
}

// Grouped data structure for hierarchical display
export interface GroupedData {
  groupKey: string;
  groupValue: any;
  groupField: string;
  groupColumnLabel: string;
  level: number;
  items: IProduct[] | GroupedData[]; // Can contain customers or sub-groups
  expanded: boolean;
  path: string;
}

interface SortConfig {
  column: string;
  direction: 'asc' | 'desc';
  type: DataType;
}

@Component({
  selector: 'app-store',
  imports: [ProductItem, PaginationModule, FormsModule, CommonModule, DragDropModule],
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
    { name: 'Price: Ascending', value: 'priceAsc' },
    { name: 'Price: Descending', value: 'priceDesc' },
  ];

  itemsPerPageOptions = [
    { value: '10', name: 'Default' },
    { name: '5 / page', value: 5 },
    { name: '10 / page', value: 10 },
    { name: '20 / page', value: 20 },
    { name: '25 / page', value: 25 },
    { name: '50 / page', value: 50 },
  ];
  toggleStyle: WritableSignal<'grid' | 'list'> = signal('grid');

  @ViewChild('searchTerm') searchTerm: ElementRef | undefined;
  totalItems = 0;
  currentPage = 1;
  itemsPerPage = 10;
  maxSize = 7; // max page buttons shown
  rotate = true; // keep current page centered
  boundaryLinks = true; // show First / Last buttons

  visible: WritableSignal<boolean> = signal(false);
  selectAll: WritableSignal<boolean> = signal(false);
  position = {
    x: 0,
    y: 0,
  };
  @ViewChild('table') table!: ElementRef<any>;
  @ViewChild('contextMenu') contextMenu!: ElementRef<HTMLDivElement>;

  columnsConfig: WritableSignal<ColumnsConfig[]> = signal([
    {
      field: 'id',
      label: 'Id',
      type: 'number',
      groupedBy: false,
      selected: false,
      width: 70,
      visible: true,
      fixed: 'left',
      disableVisiblity: true,
      rejectUnFixed: true,
    },
    {
      field: 'name',
      label: 'Name',
      type: 'string',
      groupedBy: false,
      width: 150,
      visible: true,
      fixed: 'left',
      disableVisiblity: true,
      rejectUnFixed: true,
    },
    {
      field: 'country',
      label: 'Country',
      type: 'string',
      groupedBy: false,
      operators: ['equals', 'gte', 'lte'],
      width: 150,
      visible: true,
      disableVisiblity: false,
      rejectUnFixed: false,
    },
    {
      field: 'company',
      label: 'Company',
      type: 'string',
      groupedBy: false,
      width: 150,
      visible: true,
      disableVisiblity: false,
      rejectUnFixed: false,
    },
    {
      field: 'date',
      label: 'Date',
      type: 'date',
      groupedBy: false,
      width: 150,
      visible: true,
      disableVisiblity: false,
      rejectUnFixed: false,
    },
    {
      field: 'activity',
      label: 'Activity',
      type: 'number',
      groupedBy: false,
      width: 100,
      visible: true,
      disableVisiblity: false,
      rejectUnFixed: false,
    },
    {
      field: 'price',
      label: 'Price',
      type: 'number',
      groupedBy: false,
      width: 100,
      visible: true,
      disableVisiblity: false,
      rejectUnFixed: false,
    },
  ]);

  items: ContextMenuItemsMenu[] = [];

  grouppedMode: WritableSignal<boolean> = signal(false);
  // Track which columns are grouped and their order
  groupedColumns: WritableSignal<string[]> = signal([]);

  // Store expanded/collapsed state for each group path
  expandedGroups: WritableSignal<Set<string>> = signal(new Set());

  selectedColumns: ColumnsConfig[] = [];

  // Computed grouped data based on groupedColumns
  groupedData: Signal<GroupedData[]> = computed(() => {
    const grouped = this.groupedColumns();
    if (grouped.length === 0) {
      return [];
    }
    return this.createGroupHierarchy(this.products(), grouped, 0, '');
  });

  // Check if we're in grouped mode
  isGroupedMode: Signal<boolean> = computed(() => this.groupedColumns().length > 0);

  columnWidths: Record<string, number> = {};

  sortConfig: WritableSignal<SortConfig[]> = signal<SortConfig[]>([]);

  // Computed sorted data
  sortedData = computed(() => {
    const config = this.sortConfig();
    if (config.length === 0) return this.products();

    return [...this.products()].sort((a, b) => {
      for (const sort of config) {
        const aVal = a[sort.column as keyof ColumnsConfig];
        const bVal = b[sort.column as keyof ColumnsConfig];
        const comparison = this.compareValues(aVal, bVal, sort.type);

        if (comparison !== 0) {
          return sort.direction === 'asc' ? comparison : -comparison;
        }
      }
      return 0;
    });
  });

  leftFixedColumns = computed(() =>
    this.columnsConfig()
      .filter((col) => col.fixed === 'left')
      .sort((a, b) => (a.fixedOrder || 0) - (b.fixedOrder || 0)),
  );

  rightFixedColumns = computed(() =>
    this.columnsConfig()
      .filter((col) => col.fixed === 'right')
      .sort((a, b) => (a.fixedOrder || 0) - (b.fixedOrder || 0)),
  );

  scrollableColumns = computed(() => this.columnsConfig().filter((col) => !col.fixed));

  private resizing = false;
  private startX = 0;
  private startWidth = 0;
  private currentColumn: string | null = null;

  rangeItems: Signal<string> = computed(() => `${this.startIndex()} - ${this.endIndex()}`);

  pageSize: Signal<number> = signal(50);
  startIndex: WritableSignal<number> = signal(1);
  endIndex: WritableSignal<number> = signal(50);
  pageNumber = signal(1);
  pageNumbers = computed(() => {
    // const pageSize = this.endIndex() - this.startIndex() + 1;
    return Math.ceil(this.products().length / this.pageSize());
  });

  constructor(private store: StoreService) {}

  ngOnInit(): void {
    // Initialization logic can go here
    this.getAllProducts();

    this.getAllBrands();

    this.getAllTypes();
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

  getAllBrands(): void {
    this.store.getAllBrands().subscribe({
      next: (res) => {
        console.log('brands', res);
        this.brands = [{ id: '', name: 'All Brands' }, ...res];
      },
      error: (err) => {
        console.error('Error fetching brands', err);
      },
    });
  }

  getAllTypes(): void {
    this.store.getAllTypes().subscribe({
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

  toggleViewStyle() {
    if (this.toggleStyle() === 'grid') {
      this.toggleStyle.set('list');
    } else {
      this.toggleStyle.set('grid');
    }
  }

  remove(product: IProduct) {}

  openContextMenu(event: MouseEvent, column: ColumnsConfig): void {
    event.preventDefault();

    // Context menu logic can be implemented here
    this.items = [
      {
        label: 'Group by this column',
        icon: 'pi pi-filter',
        action: () => this.addGrouping(column),
      },
      {
        label: 'Remove from grouping',
        icon: 'pi pi-filter-slash',
        action: () => this.removeGrouping(column),
      },
      {
        label: 'Clear all grouping',
        icon: 'pi pi-times',
        action: () => this.clearAllGrouping(),
      },
      { separator: true },
      {
        label: 'Sort Ascending',
        icon: 'pi pi-sort-amount-up',
        action: () => this.sort('asc', column, event),
      },
      {
        label: 'Sort Descending',
        icon: 'pi pi-sort-amount-down',
        action: () => this.sort('desc', column, event),
      },
      { separator: true },
      {
        label: 'Fix this column left',
        icon: 'pi pi-arrow-left',
        action: () => this.fixColumns(column, 'left'),
      },
      {
        label: 'Fix this column right',
        icon: 'pi pi-arrow-right',
        action: () => this.fixColumns(column, 'right'),
      },
      {
        label: 'Unfix this column',
        icon: 'pi pi-thumbtack',
        action: () => this.unFixColumn(column),
      },
      {
        label: 'Unfix all columns',
        icon: 'pi pi-thumbtack',
        action: () => this.unFixAllColumns(),
      },
      { separator: true },
      {
        label: 'Hide Column',
        icon: 'pi pi-eye-slash',
        action: () => this.hideColumn(column),
      },
    ];

    this.position.x = event.clientX;
    this.position.y = event.clientY;
    this.visible.set(true);
  }

  onColumnDrop(event: CdkDragDrop<ColumnsConfig[]>): void {
    const columns = [...this.columnsConfig()];
    moveItemInArray(columns, event.previousIndex, event.currentIndex);
    this.columnsConfig.set(columns);

    setTimeout(() => {
      this.table?.nativeElement?.reset();
    }, 0);
  }

  sort(order: 'asc' | 'desc', column: ColumnsConfig, event: MouseEvent): void {
    const updatedColumns = this.columnsConfig().map((col) =>
      col.field === column.field ? { ...col, sortedBy: order } : col,
    );
    this.columnsConfig.set(updatedColumns);

    if (event.ctrlKey || event.metaKey) {
      this.sortConfig.update((prev) => prev.filter((sort) => sort.column !== column.field));
      return;
    }

    if (event.shiftKey) {
      this.sortConfig.update((prev) => {
        const existing = prev.find((sort) => sort.column === column.field);
        if (existing) {
          return prev.map((sort) => {
            return sort.column === column.field ? { ...sort, direction: order } : sort;
          });
        } else {
          return [...prev, { column: column.field, direction: order, type: column.type }];
        }
      });
    } else {
      this.sortConfig.set([{ column: column.field, direction: order, type: column.type }]);
    }
    this.visible.set(false);
  }

  createGroupHierarchy(
    products: IProduct[],
    groupedColumns: string[],
    level: number,
    parentPath: string,
  ): GroupedData[] {
    if (groupedColumns.length === 0 || products.length === 0 || level >= groupedColumns.length) {
      return [];
    }

    const currentField = groupedColumns[level];
    const column = this.columnsConfig().find((col) => col.field === currentField);
    if (!column) {
      return [];
    }

    const groupsMap: Map<any, IProduct[]> = new Map();

    products.forEach((product) => {
      const value = this.getFieldValue(product, currentField);
      const groupKey = this.getGroupKey(value);
      // const groupKey = product[currentField as keyof IProduct];
      if (!groupsMap.has(groupKey)) {
        groupsMap.set(groupKey, []);
      }
      groupsMap.get(groupKey)!.push(product);
    });

    const groupedData: GroupedData[] = [];

    groupsMap.forEach((groupedItems, groupKey) => {
      const path = parentPath ? `${parentPath}|${groupKey}` : groupKey;
      const grouped: GroupedData = {
        groupKey: groupKey,
        groupField: currentField,
        groupColumnLabel: column.label,
        groupValue: this.getFieldValue(groupedItems[0], currentField),
        level: level,
        items:
          level === groupedColumns.length - 1
            ? groupedItems
            : this.createGroupHierarchy(
                groupedItems,
                groupedColumns,
                level + 1,
                parentPath + '|' + groupKey,
              ),
        expanded: this.expandedGroups().has(path),
        path: path,
      };

      groupedData.push(grouped);
    });

    return groupedData;
  }

  getFieldValue(product: IProduct, field: string): any {
    if (field.includes('.')) {
      return field.split('.').reduce((obj, key) => (obj as IProduct)[key], product);
    }
    return product[field as keyof IProduct];
  }

  getGroupKey(value: any): string {
    if (value == null) return '__null__';
    if (typeof value === 'object') {
      return value.name || JSON.stringify(value);
    }
    return String(value);
  }

  addGrouping(column: ColumnsConfig): void {
    this.grouppedMode.set(true);
    const updatedColumns = this.columnsConfig().map((col) =>
      col.field === column.field ? { ...col, groupedBy: true } : col,
    );
    this.columnsConfig.set(updatedColumns);

    if (!this.groupedColumns().includes(column.field)) {
      this.groupedColumns.set([...this.groupedColumns(), column.field]);
      this.sort(column.sortedBy || 'asc', column, new MouseEvent('click'));
    }
    this.visible.set(false);
  }

  removeGrouping(column: ColumnsConfig): void {
    const updatedColumns = this.columnsConfig().map((col) =>
      col.field === column.field ? { ...col, groupedBy: false } : col,
    );
    this.columnsConfig.set(updatedColumns);
    this.visible.set(false);
    this.groupedColumns.set(this.groupedColumns().filter((field) => field !== column.field));
    if (this.groupedColumns().length === 0) {
      this.grouppedMode.set(false);
    }
  }

  clearAllGrouping(): void {
    this.grouppedMode.set(false);
    const updatedColumns = this.columnsConfig().map((col) => ({ ...col, groupedBy: false }));
    this.columnsConfig.set(updatedColumns);
    this.visible.set(false);
    this.groupedColumns.set([]);
  }

  fixColumns(column: ColumnsConfig, position: 'left' | 'right' | null): void {
    this.visible.set(false);
    this.columnsConfig.update((cols) => {
      if (position === null) {
        return cols.map((col) =>
          col.field === column.field ? { ...col, fixed: null, fixedOrder: undefined } : col,
        );
      }

      const fixedCols = cols.filter((col) => col.fixed === position);
      const fixedOrder = fixedCols.length;

      return cols.map((col) =>
        col.field === column.field ? { ...col, fixed: position, fixedOrder: fixedOrder } : col,
      );
    });
  }

  unFixColumn(column: ColumnsConfig): void {
    this.visible.set(false);
    const updatedColumns = this.columnsConfig().map((col) =>
      col.field === column.field ? { ...col, fixed: null, fixedOrder: undefined } : col,
    );
    this.columnsConfig.set(updatedColumns);
  }

  unFixAllColumns(): void {
    this.visible.set(false);
    const updatedColumns = this.columnsConfig().map((col) =>
      col.rejectUnFixed
        ? col
        : {
            ...col,
            fixed: null,
            fixedOrder: undefined,
          },
    );
    this.columnsConfig.set(updatedColumns);
  }

  hideColumn(column: ColumnsConfig): void {
    this.visible.set(false);
    const updatedColumns = this.columnsConfig().map((col) =>
      col.field === column.field ? { ...col, visible: false } : col,
    );
    this.columnsConfig.set(updatedColumns);
  }

  getVisibleColumns(): ColumnsConfig[] {
    return this.columnsConfig().filter(
      (col) => col.visible !== false && !this.groupedColumns().includes(col.field),
    );
  }

  // onColumnVisiblityChange(event: MultiSelectChangeEvent | ColumnsConfig[]): void {
  //   const selectedFields =
  //     Array.isArray(event) && 'field' in event[0]
  //       ? (event as ColumnsConfig[]).map((col) => col.field)
  //       : (event as MultiSelectChangeEvent).value.map((col: ColumnsConfig) => col.field);
  //   const updatedColumns = this.columnsConfig().map((col) => ({
  //     ...col,
  //     visible: selectedFields.includes(col.field),
  //   }));
  //   this.columnsConfig.set(updatedColumns);
  // }

  // Type-aware comparison
  private compareValues(a: any, b: any, type: DataType): number {
    // Handle null/undefined
    if (a == null && b == null) return 0;
    if (a == null) return 1;
    if (b == null) return -1;

    switch (type) {
      case 'number':
        return a - b;

      case 'string':
        return a.toString().localeCompare(b.toString(), undefined, {
          numeric: true,
          sensitivity: 'base',
        });

      case 'date':
        return new Date(a).getTime() - new Date(b).getTime();

      case 'boolean':
        return a === b ? 0 : a ? -1 : 1;

      default:
        return String(a).localeCompare(String(b));
    }
  }
}
