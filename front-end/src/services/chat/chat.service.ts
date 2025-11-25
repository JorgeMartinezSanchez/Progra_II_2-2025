import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

// Servicios
import { APIdataReciever } from '../APIdataReciever';
import { EncryptationService } from '../Encryptation/encryptation.service';
import { StateService } from '../State/state.service';

// Interfaces
import { CreateChat } from '../../app/interfaces/createChat';
import { ReceiveChat } from '../../app/interfaces/receiveChat'; 
import { CreateMessage } from '../../app/interfaces/createMessage';
import { ReceiveMessage } from '../../app/interfaces/receiveMessage';
import { PublicProfile } from '../../app/interfaces/publicProfile';

@Injectable({
  providedIn: 'root'
})
export class ChatService extends APIdataReciever {

  // Caché de claves AES descifradas
  // Key: ChatId, Value: CryptoKey (AES-GCM)
  private chatKeysCache = new Map<string, CryptoKey>();

  constructor(
    http: HttpClient,
    private crypto: EncryptationService,
    private state: StateService
  ) {  
    super(http, "PrivateChat"); 
  }
  
  public getChatKey(chatId: string): CryptoKey | undefined {
    return this.chatKeysCache.get(chatId);
  }

  // =========================================================
  // 1. CREAR UN CHAT
  // =========================================================

  async createPrivateChat(targetUser: PublicProfile): Promise<ReceiveChat> {
    const myState = this.state.currentState;
    
    if (!myState.currentUser || !myState.sessionKeys) {
      throw new Error("Sesión no válida o llaves perdidas.");
    }

    // Generar clave AES y cifrarla para ambos usuarios
    const cryptoData = await this.crypto.prepareChatCreation(
      myState.currentUser.publicKey,  // String Base64
      targetUser.publicKey            // String Base64
    );

    // Armar DTO para el backend
    const dto: CreateChat = {
      accountId: targetUser.accountId,
      sendingUsername: myState.currentUser.username,
      encryptedChatKeyForMe: cryptoData.encryptedForMe,
      encryptedChatKeyForThem: cryptoData.encryptedForThem
    };

    // Enviar al backend
    const newChat = await firstValueFrom(
      this.http.post<ReceiveChat>(`${this.apiUrl}`, dto)
    );

    // ✅ Guardar clave AES en caché
    this.chatKeysCache.set(newChat.chatId, cryptoData.rawAesKey);

    return newChat;
  }

  // =========================================================
  // 2. CARGAR CHATS Y DESCIFRAR LLAVES
  // =========================================================

  async loadMyChats(): Promise<ReceiveChat[]> {
    const myId = this.state.currentState.currentUser?.accountId;
    if (!myId) throw new Error("No User ID");

    // Obtener chats del backend
    const chats = await firstValueFrom(
      this.http.get<ReceiveChat[]>(`${this.apiUrl}/all/${myId}`)
    );

    const myPrivateKey = this.state.currentState.sessionKeys?.userPrivateKey;
    if (!myPrivateKey) return chats;

    // Descifrar claves AES de los chats
    await Promise.all(chats.map(async (chat: any) => {
      const encryptedKey = chat.encryptedChatKey || chat.encryptedChatKeyForMe; 

      if (encryptedKey && !this.chatKeysCache.has(chat.chatId)) {
        try {
          const aesKey = await this.crypto.decryptChatAESKey(
            encryptedKey, 
            myPrivateKey
          );
          
          this.chatKeysCache.set(chat.chatId, aesKey);
        } catch (err) {
          console.error(`❌ Error recovering key for chat ${chat.chatId}`, err);
        }
      }
    }));

    return chats;
  }

  // =========================================================
  // 3. ENVIAR MENSAJE
  // =========================================================

  async sendMessage(chatId: string, textContent: string): Promise<any> {
    // Recuperar clave AES de caché
    let aesKey = this.chatKeysCache.get(chatId);
    
    if (!aesKey) {
      // Intentar recargar chats
      await this.loadMyChats();
      aesKey = this.chatKeysCache.get(chatId);
      
      if (!aesKey) {
        throw new Error("No se tiene la clave de cifrado de este chat.");
      }
    }

    // Cifrar mensaje
    const encryptedData = await this.crypto.encryptMessage(textContent, aesKey);

    // Armar DTO
    const myId = this.state.currentState.currentUser?.accountId;
    if (!myId) throw new Error("No user ID");

    const dto: CreateMessage = {
      chatId: chatId,
      senderId: myId,
      encryptedContent: encryptedData.encryptedContent,
      iv: encryptedData.iv
    };

    // Enviar a la API
    return firstValueFrom(
      this.http.post(`http://localhost:5053/api/Message`, dto)
    );
  }

  // =========================================================
  // 4. RECIBIR Y DESCIFRAR MENSAJES
  // =========================================================

  async getMessages(chatId: string): Promise<any[]> {
    // Obtener mensajes cifrados
    const encryptedMessages = await firstValueFrom(
      this.http.get<ReceiveMessage[]>(`http://localhost:5053/api/Message/chat/${chatId}`)
    );

    // Verificar si tenemos la clave
    let aesKey = this.chatKeysCache.get(chatId);
    if (!aesKey) {
      await this.loadMyChats();
      aesKey = this.chatKeysCache.get(chatId);
    }
    
    if (!aesKey) {
      console.warn('⚠️ No AES key for chat, returning encrypted messages');
      return encryptedMessages;
    }

    // Descifrar mensajes
    const decryptedMessages = await Promise.all(
      encryptedMessages.map(async (msg) => {
        try {
          const plainText = await this.crypto.decryptMessage(
            msg.encryptedContent, 
            msg.iv, 
            aesKey!
          );
          return { ...msg, content: plainText };
        } catch (e) {
          console.error('❌ Error decrypting message:', e);
          return { ...msg, content: '⛔ Error de descifrado' };
        }
      })
    );

    return decryptedMessages;
  }

  // =========================================================
  // 5. UTILIDADES
  // =========================================================

  /**
   * Limpia el caché de claves (útil al hacer logout)
   */
  clearCache(): void {
    this.chatKeysCache.clear();
    console.log('✅ Chat keys cache cleared');
  }

  /**
   * Obtiene todos los chat IDs en caché
   */
  getCachedChatIds(): string[] {
    return Array.from(this.chatKeysCache.keys());
  }
}