import { Component } from '@angular/core';
import { AccountService } from '../account-service';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
 title="Login"
  constructor(private acntService: AccountService) { }

  login(){
    this.acntService.login();
  }
}
