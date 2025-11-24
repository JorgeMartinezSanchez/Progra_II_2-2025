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

  async generateAccountKeys(password: string): Promise<AccountKeysResult> {
    const keyPair = await this.subtle.generateKey(
      {
        name: "RSA-OAEP",
        modulusLength: 2048,
        publicExponent: new Uint8Array([1, 0, 1]),
        hash: "SHA-256",
      },
      true,
      ["encrypt", "decrypt", "wrapKey", "unwrapKey"]
    );

    const publicKeyBuffer = await this.subtle.exportKey("spki", keyPair.publicKey);
    
    // Generamos Salt (Uint8Array)
    const salt = window.crypto.getRandomValues(new Uint8Array(16));
    
    // Pasamos el salt tal cual (Uint8Array es compatible con nuestra función corregida)
    const passwordKey = await this.deriveKeyFromPassword(password, salt);
    
    const encryptedPrivateKeyBuffer = await this.subtle.wrapKey(
      "jwk",
      keyPair.privateKey,
      passwordKey,
      { name: "AES-KW" }
    );

    return {
      publicKey: this.arrayBufferToBase64(publicKeyBuffer),
      encryptedPrivateKey: this.arrayBufferToBase64(encryptedPrivateKeyBuffer),
      salt: this.arrayBufferToBase64(salt)
    };
  }

  async unlockPrivateKey(password: string, saltBase64: string, encryptedPrivateKeyBase64: string): Promise<CryptoKey> {
    const saltBuffer = this.base64ToArrayBuffer(saltBase64);
    // Convertimos a Uint8Array para que deriveKeyFromPassword lo acepte sin problemas
    const salt = new Uint8Array(saltBuffer);

    const passwordKey = await this.deriveKeyFromPassword(password, salt);
    const encryptedKeyBuffer = this.base64ToArrayBuffer(encryptedPrivateKeyBase64);

    return await this.subtle.unwrapKey(
      "jwk",
      encryptedKeyBuffer,
      passwordKey,
      { name: "AES-KW" },
      { name: "RSA-OAEP", hash: "SHA-256" },
      true,
      ["decrypt", "unwrapKey"]
    );
  }

  // ==========================================
  // 2. GESTIÓN DE CHATS
  // ==========================================

  async prepareChatCreation(myPublicKey: CryptoKey, theirPublicKeyBase64: string): Promise<ChatInitResult> {
    const theirPublicKey = await this.importPublicKey(theirPublicKeyBase64);

    const chatAesKey = await this.subtle.generateKey(
      { name: "AES-GCM", length: 256 },
      true,
      ["encrypt", "decrypt"]
    );

    const rawAesKeyBuffer = await this.subtle.exportKey("raw", chatAesKey);

    const encryptedForMeBuffer = await this.subtle.encrypt(
      { name: "RSA-OAEP" },
      myPublicKey,
      rawAesKeyBuffer
    );

    const encryptedForThemBuffer = await this.subtle.encrypt(
      { name: "RSA-OAEP" },
      theirPublicKey,
      rawAesKeyBuffer
    );

    return {
      rawAesKey: chatAesKey,
      encryptedForMe: this.arrayBufferToBase64(encryptedForMeBuffer),
      encryptedForThem: this.arrayBufferToBase64(encryptedForThemBuffer)
    };
  }

  async decryptChatAESKey(encryptedChatKeyBase64: string, myPrivateKey: CryptoKey): Promise<CryptoKey> {
    const encryptedBuffer = this.base64ToArrayBuffer(encryptedChatKeyBase64);
    
    const rawAesKeyBuffer = await this.subtle.decrypt(
      { name: "RSA-OAEP" },
      myPrivateKey,
      encryptedBuffer
    );

    return await this.subtle.importKey(
      "raw",
      rawAesKeyBuffer,
      { name: "AES-GCM" },
      true,
      ["encrypt", "decrypt"]
    );
  }

  // ==========================================
  // 3. MENSAJERÍA
  // ==========================================

  async encryptMessage(text: string, chatAesKey: CryptoKey): Promise<MessagePayload> {
    const iv = window.crypto.getRandomValues(new Uint8Array(12)); 
    const encodedText = new TextEncoder().encode(text);

    const ciphertextBuffer = await this.subtle.encrypt(
      { name: "AES-GCM", iv: iv },
      chatAesKey,
      encodedText
    );

    return {
      encryptedContent: this.arrayBufferToBase64(ciphertextBuffer),
      iv: this.arrayBufferToBase64(iv)
    };
  }

  async decryptMessage(encryptedContentBase64: string, ivBase64: string, chatAesKey: CryptoKey): Promise<string> {
    const ciphertext = this.base64ToArrayBuffer(encryptedContentBase64);
    const iv = this.base64ToArrayBuffer(ivBase64);

    try {
      const decryptedBuffer = await this.subtle.decrypt(
        { name: "AES-GCM", iv: iv },
        chatAesKey,
        ciphertext
      );
      return new TextDecoder().decode(decryptedBuffer);
    } catch (e) {
      console.error("Error descifrando mensaje:", e);
      return "[Mensaje ilegible]";
    }
  }

  // ==========================================
  // 4. UTILIDADES
  // ==========================================

  private async deriveKeyFromPassword(password: string, salt: Uint8Array): Promise<CryptoKey> {
    const passwordBuffer = new TextEncoder().encode(password);
    const importedPassword = await this.subtle.importKey(
      "raw",
      passwordBuffer,
      "PBKDF2",
      false,
      ["deriveKey"]
    );

    return await this.subtle.deriveKey(
      {
        name: "PBKDF2",
        // SOLUCIÓN ERROR 2: Usamos 'as any' para evitar el error de tipado 
        // estricto de TypeScript sobre SharedArrayBuffer.
        // El navegador aceptará el Uint8Array sin problemas.
        salt: salt as any, 
        iterations: 100000,
        hash: "SHA-256"
      },
      importedPassword,
      { name: "AES-KW", length: 256 },
      true,
      ["wrapKey", "unwrapKey"]
    );
  }

  private async importPublicKey(base64Key: string): Promise<CryptoKey> {
    const binaryKey = this.base64ToArrayBuffer(base64Key);
    return await this.subtle.importKey(
      "spki",
      binaryKey,
      { name: "RSA-OAEP", hash: "SHA-256" },
      true,
      ["encrypt"] 
    );
  }

  // Acepta BufferSource para manejar tanto ArrayBuffer como Uint8Array
  private arrayBufferToBase64(buffer: BufferSource): string {
    let bytes: Uint8Array;

    if (buffer instanceof Uint8Array) {
      bytes = buffer;
    } else {
      bytes = new Uint8Array(buffer as ArrayBuffer);
    }

    let binary = '';
    const len = bytes.byteLength;
    for (let i = 0; i < len; i++) {
      binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
  }

  private base64ToArrayBuffer(base64: string): ArrayBuffer {
    const binaryString = window.atob(base64);
    const len = binaryString.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
      bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes.buffer;
  }
}