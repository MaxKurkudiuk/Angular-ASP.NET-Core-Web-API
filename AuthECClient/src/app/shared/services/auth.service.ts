import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { TOKEN_KEY } from '../constants';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);

  createUser(formData: any) {
    // WARNING!
    // default value for Role, Gender, Age, LibraryID?
    // instead of registration form, there should be some other
    // form to update these details of the user
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
  }

  deleteToken() {
    localStorage.removeItem(TOKEN_KEY);
  }
}
