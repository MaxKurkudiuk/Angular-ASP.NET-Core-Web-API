import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private http = inject(HttpClient);

  getUserProfile() {
    return this.http.get(environment.apiBaseUrl + '/userprofile');
  }

  updateProfile(profile: { fullName?: string; age?: number; gender?: string; libraryID?: string }) {
    return this.http.put(environment.apiBaseUrl + '/userprofile', profile);
  }
}
