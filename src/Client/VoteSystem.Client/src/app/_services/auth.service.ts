import { inject, Injectable, signal } from '@angular/core';
import { UserToken } from '../_models/userToken';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { UserLoginRequest } from '../_models/_contracts/userLoginRequest';
import { UserRegisterRequest } from '../_models/_contracts/userRegisterRequest';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private client = inject(HttpClient);
  private baseUrl = environment.authBaseUrl;

  currentUser = signal<UserToken | null>(null);

  loginUser(userLoginRequest: UserLoginRequest) {
    return this.client
      .post<UserToken>(this.baseUrl + 'auth/login', userLoginRequest)
      .pipe(tap((x) => this.setCurrentUser(x)));
  }
  registerUser(userRegisterRequest: UserRegisterRequest) {
    return this.client
      .post<UserToken>(this.baseUrl + 'auth/register', userRegisterRequest)
      .pipe(tap((x) => this.setCurrentUser(x)));
  }
  registerAdmin(userRegisterRequest: UserRegisterRequest) {
    return this.client
      .post<UserToken>(
        this.baseUrl + 'auth/register/admin',
        userRegisterRequest
      )
      .pipe(tap((x) => this.setCurrentUser(x)));
  }
  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }
  setCurrentUser(user: UserToken) {
    var userJson = JSON.stringify(user);

    localStorage.setItem('user', userJson);

    if (!this.currentUser()) {
      this.currentUser.set(user);
    }
  }
  constructor() {}
}
