import { Injectable } from '@angular/core';
import { APIdataReciever } from '../APIdataReciever';
import { HttpClient } from '@angular/common/http';
import { Account } from '../../app/interfaces/account';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService extends APIdataReciever {

  constructor(http: HttpClient) { 
    super(http);
  }

  /*createAccount(newAccount: Account): Observable<Account>{
    return this.http.post(`${this.apiUrl}/`, { newAccount });
  }*/
}
