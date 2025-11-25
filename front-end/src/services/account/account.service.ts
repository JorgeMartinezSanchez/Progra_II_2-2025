import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, from, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
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
    
    // Validaciones mejoradas
    if (!username || !password) {
      return throwError(() => new Error('Username and password are required'));
    }

    if (password.length < 8) {
      return throwError(() => new Error('Password must be at least 8 characters long'));
    }

    console.log('👤 Starting user registration...');
    console.log('Username:', username);
    console.log('Password length:', password.length);
    console.log('Profile pic base64 length:', base64Pfp?.length || 0);

    return from(this.crypto.generateAccountKeys(password)).pipe(
      switchMap(keys => {
        console.log('✅ Account keys generated successfully');
        console.log('Keys structure:', {
          publicKeyLength: keys.publicKey.length,
          encryptedPrivateKeyLength: keys.encryptedPrivateKey.length,
          saltLength: keys.salt.length
        });

        const newAccount: CreateAccount = {
          username: username,
          base64Pfp: base64Pfp,
          publicKey: keys.publicKey,
          encryptedPrivateKey: keys.encryptedPrivateKey,
          salt: keys.salt
        };
        
        console.log('📦 Sending account data to server...');
        return this.createAccount(newAccount);
      }),
      catchError(error => {
        console.error('❌ Error in registerUser:', error);
        
        // Mensajes de error más específicos
        let errorMessage = 'Registration failed';
        if (error.message.includes('AES-KW')) {
          errorMessage = 'Security error: Invalid key format. Please try again.';
        } else if (error.name === 'OperationError') {
          errorMessage = 'Cryptographic operation failed. Please check your password.';
        } else {
          errorMessage = `Registration error: ${error.message}`;
        }
        
        return throwError(() => new Error(errorMessage));
      })
    );
  }

  private createAccount(newAccount: CreateAccount): Observable<RecieveAccount>{
    return this.http.post<RecieveAccount>(`${this.apiUrl}/`, newAccount);
  }
}
