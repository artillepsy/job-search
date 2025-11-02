import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoginService {
  private _userToken?: string = localStorage.getItem('token') ?? undefined;

  get userToken() {
    return this._userToken;
  } 

  logIn(username: string, password: string) {
    console.log(`login. Username: ${username}, password: ${password}`);
    this._userToken = username;
    localStorage.setItem('token', this._userToken);
    return true;
  }

  logOut() {
    console.log("logout");
    this._userToken = undefined;
    localStorage.removeItem('token');
  }
}
