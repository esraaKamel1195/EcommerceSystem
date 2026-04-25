import { inject, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, NavigationEnd, Router } from '@angular/router';
import { filter, map, Observable, startWith } from 'rxjs';

export interface BreadcrumbItem {
  label: string;
  url: string;
  current: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class Breadcrumb {
  private router = inject(Router);

  readonly breadcrumb$: Observable<BreadcrumbItem[]> = this.router.events.pipe(
    filter((event): event is NavigationEnd => event instanceof NavigationEnd),
    startWith(null), // Emit an initial value to build breadcrumbs on app load
    map(() => this.buildCrumbs(this.router.routerState.snapshot.root, '', [])),
  );

  private buildCrumbs(
    route: ActivatedRouteSnapshot,
    url: string,
    acc: BreadcrumbItem[],
  ): BreadcrumbItem[] {
    const segment = route.url.map((s) => s.path).join('/');
    const nextUrl = segment ? `${url}/${segment}` : url;

    if (route.data['breadcrumb']) {
      const label = this.resolveLabel(route.data['breadcrumb'], route);
      acc.push({ label, url: nextUrl || '/', current: false });
    }

    if (route.firstChild) {
      return this.buildCrumbs(route.firstChild, nextUrl, acc);
    }

    // Mark last crumb as current
    if (acc.length) acc[acc.length - 1].current = true;
    return acc;
  }

  private resolveLabel(
    crumb: string | ((data: any) => string),
    route: ActivatedRouteSnapshot,
  ): string {
    return typeof crumb === 'function' ? crumb(route.data) : crumb;
  }
}
