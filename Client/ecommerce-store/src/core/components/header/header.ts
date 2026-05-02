import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Breadcrumb } from '../../services/breadcrumb';

@Component({
  selector: 'app-header',
  imports: [CommonModule, RouterModule],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header implements OnInit {
  protected crumbs$ = inject(Breadcrumb).breadcrumb$;

  ngOnInit(): void {
    console.log('Crumbs:', this.crumbs$);
  }
}
