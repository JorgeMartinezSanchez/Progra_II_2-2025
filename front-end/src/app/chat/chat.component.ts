import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Input, Output, OnChanges, SimpleChanges, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ReceiveChat } from '../interfaces/receiveChat';
import { ReceiveMessage } from '../interfaces/receiveMessage';
import { MessageService } from '../../services/message/message.service';
import { StateService } from '../../services/State/state.service';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css'
})
export class ChatComponent implements OnChanges, OnInit {
  private router = inject(Router);
  private messageService = inject(MessageService);
  private stateService = inject(StateService);
  
  public chatIsOpen = false;
  public currentChat: ReceiveChat | null = null;
  public messages: ReceiveMessage[] = [];
  public loadingMessages = false;
  
  @Input() chatData: ReceiveChat | null = null;
  @Input() userMap: { [key: string]: string } = {}; // Cambiado a string para accountId
  @Output() chatOutput = new EventEmitter<ReceiveChat>();

  ngOnInit() {
    // Suscribirse a cambios de estado por si se cierra sesión
    this.stateService.state$.subscribe(state => {
      if (!state.currentUser) {
        this.currentChat = null;
        this.chatIsOpen = false;
        this.messages = [];
      }
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['chatData'] && changes['chatData'].currentValue) {
      this.currentChat = changes['chatData'].currentValue;
      this.chatIsOpen = true;
      console.log('Chat cargado:', this.currentChat);
      this.loadMessages();
    } else {
      this.currentChat = null;
      this.chatIsOpen = false;
      this.messages = [];
    }
  }

  async loadMessages() {
    if (!this.currentChat?.chatId) return;
    
    try {
      this.loadingMessages = true;
      this.messages = await this.messageService.getMessagesByChat(this.currentChat.chatId);
      console.log('Mensajes cargados:', this.messages);
    } catch (error) {
      console.error('Error cargando mensajes:', error);
    } finally {
      this.loadingMessages = false;
    }
  }

  // Método para obtener el nombre del remitente
  getSenderName(message: ReceiveMessage): string {
    // Si el mensaje es del usuario actual, no mostrar nombre
    if (message.senderId === this.stateService.currentState.currentUser?.accountId) {
      return '';
    }
    
    // Usar el userMap que viene del home component
    return this.userMap[message.senderId] || `Usuario ${message.senderId}`;
  }

  // Verificar si el mensaje es propio
  isOwnMessage(message: ReceiveMessage): boolean {
    return message.senderId === this.stateService.currentState.currentUser?.accountId;
  }

  goBack() {
    this.router.navigate(['/']);
  }

  async sendMessage(message: string) {
    if (this.currentChat && message.trim()) {
      try {
        await this.messageService.sendMessage(this.currentChat.chatId, message);
        
        // Recargar mensajes para mostrar el nuevo
        await this.loadMessages();
      } catch (error) {
        console.error('Error enviando mensaje:', error);
      }
    }
  }

  // Formatear fecha para mostrar
  formatMessageTime(timestamp: string): string {
    return new Date(timestamp).toLocaleTimeString('es-ES', { 
      hour: '2-digit', 
      minute: '2-digit' 
    });
  }
}