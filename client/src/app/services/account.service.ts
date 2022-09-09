import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseURL = 'https://localhost:5001/api/';
  users: any;

  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post(this.baseURL + 'account/login', model).subscribe({
      next: response => console.log(response),
      error: error => console.log(error)
    });
  }
}
