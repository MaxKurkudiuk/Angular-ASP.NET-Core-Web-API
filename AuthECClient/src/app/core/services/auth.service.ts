import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { TOKEN_KEY } from '../constants/constants';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  claims = signal<any>(this.getClaimsOrNull());

  createUser(formData: any) {
    // Multi-step registration: Step 1 collects FullName, Email, Password.
    // Step 2 collects Gender, Age, LibraryID. Everything is submitted together.
    // Role defaults to "Student" on the server.
    return this.http.post(environment.apiBaseUrl + '/signup', formData);
  }

  signin(formData: any) {
    return this.http.post(environment.apiBaseUrl + '/signin', formData);
  }

  isLoggedIn() {
    return this.getToken() != null ? true : false;
  }

  getToken() {
    return localStorage.getItem(TOKEN_KEY);
  }

  saveToken(token: string) {
    localStorage.setItem(TOKEN_KEY, token);
    this.claims.set(this.getClaimsOrNull());
  }

  deleteToken() {
    localStorage.removeItem(TOKEN_KEY);
    this.claims.set(null);
  }

  getClaims() {
    return JSON.parse(window.atob(this.getToken()!.split('.')[1]));
  }

  private getClaimsOrNull() {
    try {
      const token = this.getToken();
      return token ? JSON.parse(window.atob(token.split('.')[1])) : null;
    } catch {
      return null;
    }
  }
}
