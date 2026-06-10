import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  getUserProfile() {
    const reqHeader = new HttpHeaders()
      .set('Authorization', 'Bearer ' + this.authService.getToken());
    return this.http.get(environment.apiBaseUrl + '/userprofile', { headers: reqHeader });
  }
}
