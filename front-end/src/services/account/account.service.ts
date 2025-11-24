import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, from } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { APIdataReciever } from '../APIdataReciever'; // Asumo que es tu clase base
import { CreateAccount } from '../../app/interfaces/createAccount'; //
import { RecieveAccount } from '../../app/interfaces/receiveAccount'; //
import { EncryptationService } from '../Encryptation/encryptation.service'; // Tu servicio de encriptación

@Injectable({
  providedIn: 'root'
})
export class AccountService extends APIdataReciever {

  constructor(
    http: HttpClient,
    private crypto: EncryptationService
  ) {  
    super(http, "Account");
  }

  public registerUser(username: string, password: string, base64Pfp: string): Observable<RecieveAccount> {
    
    // Usamos 'from' para convertir la Promesa del crypto en Observable
    return from(this.crypto.generateAccountKeys(password)).pipe(
      switchMap(keys => {
        const newAccount: CreateAccount = {
          username: username,
          base64pfp: base64Pfp,
          publicKey: keys.publicKey,
          encryptedPrivateKey: keys.encryptedPrivateKey,
          salt: keys.salt
        };
        return this.createAccount(newAccount);
      })
    );
  }

  private createAccount(newAccount: CreateAccount): Observable<RecieveAccount>{
    return this.http.post<RecieveAccount>(`${this.apiUrl}/`, newAccount);
  }
}
