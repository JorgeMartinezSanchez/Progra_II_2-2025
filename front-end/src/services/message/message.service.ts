import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

import { APIdataReciever } from '../APIdataReciever';
import { EncryptationService } from '../Encryptation/encryptation.service';
import { StateService } from '../State/state.service';
import { ChatService } from '../chat/chat.service';

import { CreateMessage } from '../../app/interfaces/createMessage';
import { ReceiveMessage } from '../../app/interfaces/receiveMessage';

@Injectable({
  providedIn: 'root'
})
export class MessageService extends APIdataReciever {

  constructor(
    http: HttpClient,
    private crypto: EncryptationService,
    private state: StateService,
    private chatService: ChatService // <--- Importante
  ) {  
    super(http, "Message"); 
  }

  // ==========================================
  // 1. OBTENER MENSAJES (Lógica movida y corregida)
  // ==========================================
  
  async getMessagesByChat(chatId: string): Promise<ReceiveMessage[]> {
    // A. Obtener mensajes cifrados del backend
    const encryptedMessages = await firstValueFrom(
      this.http.get<ReceiveMessage[]>(`${this.apiUrl}/chat/${chatId}`)
    );

    // B. Obtener la llave AES del ChatService
    let aesKey = this.chatService.getChatKey(chatId);

    // Si no hay llave en caché, intentamos forzar una recarga de los chats
    // (Por si el usuario acaba de dar F5 y el ChatService está vacío)
    if (!aesKey) {
      console.log("Llave no encontrada en caché, recargando chats...");
      await this.chatService.loadMyChats();
      aesKey = this.chatService.getChatKey(chatId);
    }

    // Si aún no hay llave, devolvemos los mensajes cifrados (fallback de seguridad)
    if (!aesKey) {
      console.warn(`No se tiene la llave para el chat ${chatId}. Imposible descifrar.`);
      return encryptedMessages;
    }

    // C. Descifrar mensajes en paralelo
    const decryptedMessages = await Promise.all(encryptedMessages.map(async (msg) => {
      try {
        const plainText = await this.crypto.decryptMessage(
          msg.encryptedContent,
          msg.iv,
          aesKey! // Usamos '!' porque ya verificamos que existe arriba
        );
        
        // Retornamos el mensaje con el contenido descifrado
        // (Asegúrate de que tu interfaz ReceiveMessage tenga la propiedad opcional 'content' o similar)
        return { ...msg, content: plainText }; 
      } catch (error) {
        console.error("Error descifrando mensaje individual", msg.messageId);
        return { ...msg, content: '🔒 Mensaje ilegible' };
      }
    }));

    // Hacemos cast a any[] para evitar conflictos si la interfaz no tiene 'content' explícito todavía
    return decryptedMessages as any[]; 
  }

  // ==========================================
  // 2. ENVIAR MENSAJE
  // ==========================================

  async sendMessage(chatId: string, textContent: string): Promise<ReceiveMessage> {
    const myId = this.state.currentState.currentUser?.accountId;
    if (!myId) throw new Error("No estás logueado");

    const aesKey = this.chatService.getChatKey(chatId);
    if (!aesKey) throw new Error("Error crítico: No se encuentra la llave de cifrado para este chat.");

    const cryptoData = await this.crypto.encryptMessage(textContent, aesKey);

    const dto: CreateMessage = {
      chatId: chatId,
      senderId: myId,
      encryptedContent: cryptoData.encryptedContent,
      iv: cryptoData.iv
    };

    return await firstValueFrom(
      this.http.post<ReceiveMessage>(`${this.apiUrl}`, dto)
    );
  }

  // ==========================================
  // 3. MARCAR COMO VISTO (PUT api/Message/mark-seen/{chatId})
  // ==========================================

  async markAsSeen(chatId: string): Promise<void> {
    await firstValueFrom(
      this.http.put<void>(`${this.apiUrl}/mark-seen/${chatId}`, {})
    );
  }

  // ==========================================
  // 4. BORRAR MENSAJES (DELETE api/Message)
  // ==========================================

  /**
   * Nota: Tu backend espera una LISTA de ReceiveMessageDto en el body del DELETE.
   * Angular HttpClient requiere configurar el 'body' explícitamente en peticiones DELETE.
   */
  async deleteMessages(messages: ReceiveMessage[]): Promise<void> {
    if (!messages || messages.length === 0) return;

    await firstValueFrom(
      this.http.request('delete', `${this.apiUrl}`, {
        body: messages // Enviamos la lista completa como pide tu MessageController
      })
    );
  }
}
