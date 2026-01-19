import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import {environment} from '../../../../environments/environment';

export interface LoginResponse {
  ok: boolean;
  userId?: string;
  username?: string;
  token?: string;
}

@Injectable({
  providedIn: 'root',
})
export class LoginService {
  private _user?: LoginResponse;
  private _http = inject(HttpClient);

  get username() {
    return this._user?.username;
  }

  get userToken() {
    return this._user?.token;
  }

  logIn(username: string, password: string) {
    const url = `${environment.apiUrl}/users/login`;
    this._http.post<LoginResponse>(url, {username, password}).subscribe({
      next: (response) => {
        this._user = response;
      },
      error: (err) => {
        console.error('Login error', err);
      },
      complete: () => {
        console.log('complete');
      },
    });

    console.log(`login. Username: ${username}, password: ${password}`);
    return true;
  }


  logOut() {
    console.log("logout");
    // this._userToken = undefined;
    localStorage.removeItem('token');
  }
}
