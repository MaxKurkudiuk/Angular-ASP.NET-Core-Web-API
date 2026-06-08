import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { TOKEN_KEY } from '../constants';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
    private http = inject(HttpClient);
    baseUrl = 'http://localhost:5292/api';

    createUser(formData:any){
        return this.http.post(this.baseUrl + '/signup', formData);
    }

    signin(formData:any){
        return this.http.post(this.baseUrl + '/signin', formData);
    }

    isLoggedIn(){
        return localStorage.getItem(TOKEN_KEY) != null ? true : false;
    }
}
