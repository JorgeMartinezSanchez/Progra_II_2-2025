import { Injectable } from '@angular/core';
import { AccountKeysResult } from '../../app/interfaces/accountKeyResult';
import { ChatInitResult } from '../../app/interfaces/chatInitResult';
import { MessagePayload } from '../../app/interfaces/messagePayLoad'; 

@Injectable({
  providedIn: 'root'
})
export class EncryptationService {

  private subtle = window.crypto.subtle;

  constructor() { }

  // ==========================================
  // 1. GESTIÓN DE CUENTA
  // ==========================================

  /**
   * Genera par de claves RSA y las protege con contraseña
   * @returns Todo en formato Base64 listo para enviar al backend
   */
  async generateAccountKeys(password: string): Promise<AccountKeysResult> {
    try {
      console.log('🔐 Generating RSA key pair...');
      
      const keyPair = await this.subtle.generateKey(
        {
          name: 'RSA-OAEP',
          modulusLength: 2048,
          publicExponent: new Uint8Array([1, 0, 1]),
          hash: 'SHA-256',
        },
        true,
        ['encrypt', 'decrypt']
      );

      const publicKeyBuffer = await this.subtle.exportKey('spki', keyPair.publicKey);
      const privateKeyBuffer = await this.subtle.exportKey('pkcs8', keyPair.privateKey);

      const salt = window.crypto.getRandomValues(new Uint8Array(16));

      const encryptedPrivateKey = await this.encryptPrivateKeyWithPassword(
        privateKeyBuffer,
        password,
        salt
      );

      console.log('✅ Keys generated successfully');

      return {
        publicKey: this.toBase64(publicKeyBuffer),
        encryptedPrivateKey: encryptedPrivateKey,
        salt: this.toBase64(salt)
      };
    } catch (error) {
      console.error('❌ Error generating keys:', error);
      throw new Error(`Key generation failed: ${String(error)}`);
    }
  }

  /**
   * Desbloquea la clave privada RSA usando la contraseña
   * @returns CryptoKey listo para descifrar
   */
  async unlockPrivateKey(
    password: string,
    saltBase64: string,
    encryptedPrivateKeyBase64: string
  ): Promise<CryptoKey> {
    try {
      console.log('🔓 Unlocking private key...');

      const salt = this.fromBase64(saltBase64);
      const encryptedData = this.fromBase64(encryptedPrivateKeyBase64);

      const iv = encryptedData.slice(0, 12);
      const ciphertext = encryptedData.slice(12);

      const derivedKey = await this.deriveKeyFromPassword(password, salt);

      const ctBuf = this.toArrayBuffer(ciphertext);
      const ivBuf = this.toArrayBuffer(iv);

      const decryptedBuffer = await this.subtle.decrypt(
        { name: 'AES-GCM', iv: ivBuf },
        derivedKey,
        ctBuf
      );

      const privateKey = await this.subtle.importKey(
        'pkcs8',
        decryptedBuffer,
        { name: 'RSA-OAEP', hash: 'SHA-256' },
        true,
        ['decrypt']
      );

      console.log('✅ Private key unlocked');
      return privateKey;
    } catch (error) {
      console.error('❌ Failed to unlock private key:', error);
      throw new Error('Invalid password or corrupted key');
    }
  }

  // ==========================================
  // 2. GESTIÓN DE CHATS
  // ==========================================

  async prepareChatCreation(
    myPublicKeyBase64: string,
    theirPublicKeyBase64: string
  ): Promise<ChatInitResult> {
    try {
      const myPublicKey = await this.importPublicKeyFromBase64(myPublicKeyBase64);
      const theirPublicKey = await this.importPublicKeyFromBase64(theirPublicKeyBase64);

      const chatAesKey = await this.subtle.generateKey(
        { name: 'AES-GCM', length: 256 },
        true,
        ['encrypt', 'decrypt']
      );

      const rawAesKeyBuffer = await this.subtle.exportKey('raw', chatAesKey);

      const encryptedForMeBuffer = await this.subtle.encrypt(
        { name: 'RSA-OAEP' },
        myPublicKey,
        rawAesKeyBuffer
      );

      const encryptedForThemBuffer = await this.subtle.encrypt(
        { name: 'RSA-OAEP' },
        theirPublicKey,
        rawAesKeyBuffer
      );

      console.log('✅ Chat keys prepared');

      return {
        rawAesKey: chatAesKey,
        encryptedForMe: this.toBase64(encryptedForMeBuffer),
        encryptedForThem: this.toBase64(encryptedForThemBuffer)
      };
    } catch (error) {
      console.error('❌ Failed to prepare chat:', error);
      throw new Error('Failed to prepare chat creation');
    }
  }

  async decryptChatAESKey(
    encryptedChatKeyBase64: string,
    myPrivateKey: CryptoKey
  ): Promise<CryptoKey> {
    try {
      const encryptedBuffer = this.fromBase64(encryptedChatKeyBase64);
      const rawAesKeyBuffer = await this.subtle.decrypt(
        { name: 'RSA-OAEP' },
        myPrivateKey,
        this.toArrayBuffer(encryptedBuffer)
      );

      return await this.subtle.importKey(
        'raw',
        rawAesKeyBuffer,
        { name: 'AES-GCM' },
        true,
        ['encrypt', 'decrypt']
      );
    } catch (error) {
      console.error('❌ Failed to decrypt chat key:', error);
      throw new Error('Failed to decrypt chat key');
    }
  }

  // ==========================================
  // 3. MENSAJERÍA
  // ==========================================

  async encryptMessage(text: string, chatAesKey: CryptoKey): Promise<MessagePayload> {
    try {
      const iv = window.crypto.getRandomValues(new Uint8Array(12));
      const encodedText = new TextEncoder().encode(text);

      const ciphertextBuffer = await this.subtle.encrypt(
        { name: 'AES-GCM', iv: iv },
        chatAesKey,
        encodedText
      );

      return {
        encryptedContent: this.toBase64(ciphertextBuffer),
        iv: this.toBase64(iv)
      };
    } catch (error) {
      console.error('❌ Failed to encrypt message:', error);
      throw new Error('Failed to encrypt message');
    }
  }

  async decryptMessage(
    encryptedContentBase64: string,
    ivBase64: string,
    chatAesKey: CryptoKey
  ): Promise<string> {
    try {
      const ciphertext = this.fromBase64(encryptedContentBase64);
      const iv = this.fromBase64(ivBase64);

      const decryptedBuffer = await this.subtle.decrypt(
        { name: 'AES-GCM', iv: this.toArrayBuffer(iv) },
        chatAesKey,
        this.toArrayBuffer(ciphertext)
      );

      return new TextDecoder().decode(decryptedBuffer);
    } catch (error) {
      console.error('❌ Failed to decrypt message:', error);
      return '[Mensaje ilegible - clave incorrecta]';
    }
  }

  // ==========================================
  // 4. UTILIDADES PRIVADAS
  // ==========================================

  private async encryptPrivateKeyWithPassword(
    privateKeyBuffer: ArrayBuffer,
    password: string,
    salt: Uint8Array
  ): Promise<string> {
    const derivedKey = await this.deriveKeyFromPassword(password, salt);
    const iv = window.crypto.getRandomValues(new Uint8Array(12));

    const encryptedBuffer = await this.subtle.encrypt(
      { name: 'AES-GCM', iv: this.toArrayBuffer(iv) },
      derivedKey,
      privateKeyBuffer
    );

    const combined = new Uint8Array(iv.length + encryptedBuffer.byteLength);
    combined.set(iv, 0);
    combined.set(new Uint8Array(encryptedBuffer), iv.length);

    return this.toBase64(combined);
  }

  private async deriveKeyFromPassword(
    password: string,
    salt: Uint8Array
  ): Promise<CryptoKey> {
    const passwordBuffer = new TextEncoder().encode(password);

    const importedPassword = await this.subtle.importKey(
      'raw',
      passwordBuffer,
      'PBKDF2',
      false,
      ['deriveKey']
    );

    return await this.subtle.deriveKey(
      {
        name: 'PBKDF2',
        salt: this.toArrayBuffer(salt),
        iterations: 100000,
        hash: 'SHA-256'
      },
      importedPassword,
      { name: 'AES-GCM', length: 256 },
      false,
      ['encrypt', 'decrypt']
    );
  }

  private async importPublicKeyFromBase64(base64Key: string): Promise<CryptoKey> {
    const binaryKey = this.fromBase64(base64Key);

    return await this.subtle.importKey(
      'spki',
      this.toArrayBuffer(binaryKey),
      { name: 'RSA-OAEP', hash: 'SHA-256' },
      true,
      ['encrypt']
    );
  }

  // ==========================================
  // 5. CONVERSIONES BASE64 (ROBUSTAS)
  // ==========================================

  private toBase64(buffer: ArrayBuffer | Uint8Array): string {
    const bytes = buffer instanceof Uint8Array ? buffer : new Uint8Array(buffer);
    const chunkSize = 0x8000; // safe chunk size for apply
    let binary = '';
    for (let i = 0; i < bytes.length; i += chunkSize) {
      binary += String.fromCharCode.apply(null, Array.from(bytes.subarray(i, i + chunkSize)));
    }
    return window.btoa(binary);
  }

  private fromBase64(base64: string): Uint8Array {
    const binaryString = window.atob(base64);
    const bytes = new Uint8Array(binaryString.length);
    for (let i = 0; i < binaryString.length; i++) {
      bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes;
  }

  private toArrayBuffer(u8: Uint8Array): ArrayBuffer {
    // Return the exact underlying ArrayBuffer slice for the view
    return u8.buffer.slice(u8.byteOffset, u8.byteOffset + u8.byteLength) as ArrayBuffer;
  }

  public async importPublicKey(base64Key: string): Promise<CryptoKey> {
    return this.importPublicKeyFromBase64(base64Key);
  }
}