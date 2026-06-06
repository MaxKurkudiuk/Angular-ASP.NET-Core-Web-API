import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuchService {
    private http = inject(HttpClient);
    baseUrl = 'http://localhost:5292/api';

    createUser(formData:any){
        return this.http.post(this.baseUrl + '/signup', formData);
    }
}
