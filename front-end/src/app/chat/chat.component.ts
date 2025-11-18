import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { ChatTest } from '../interfaces/testing_interfaces/chatTest';
import { GroupTest } from '../interfaces/testing_interfaces/groupTest';
import { privateChatTest } from '../interfaces/testing_interfaces/privateChatTest';
import { MessageTest } from '../interfaces/testing_interfaces/messageTest';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css'
})
export class ChatComponent implements OnChanges {
  private router = inject(Router);
  public chatIsOpen = false;
  public currentChat: ChatTest | null = null;

  @Input() usuario!: ChatTest | null;
  @Input() userMap: { [key: number]: string } = {}; // <-- NUEVO INPUT
  @Output() chatOutput = new EventEmitter<ChatTest>();

  // Type Guards
  isGroupChat(chat: ChatTest | null): chat is GroupTest {
    return chat !== null && chat.type === 'group' && 'members' in chat;
  }

  isPrivateChat(chat: ChatTest | null): chat is privateChatTest {
    return chat !== null && chat.type === 'private' && 'emisorUser' in chat && 'receptorUser' in chat;
  }

  // Método para obtener el nombre del remitente (solo para grupos)
  getSenderName(message: MessageTest, chat: ChatTest): string {
    if (this.isGroupChat(chat)) {
      // Usa el userMap que viene del home component
      return this.userMap[message.userId] || `Usuario ${message.userId}`;
    }
    return ''; // En chats privados no mostramos nombre
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['usuario'] && changes['usuario'].currentValue) {
      this.currentChat = changes['usuario'].currentValue;
      this.chatIsOpen = true;
      console.log('Chat cargado:', this.currentChat);
    } else {
      this.currentChat = null;
      this.chatIsOpen = false;
    }
  }

  goBack() {
    this.router.navigate(['/']);
  }

  sendMessage(message: string) {
    if (this.currentChat && message.trim()) {
      const newMessage = {
        userId: 1, // ID del usuario actual (Jaime)
        message: message,
        sendDate: new Date()
      };
      
      this.currentChat.messages.push(newMessage);
      this.currentChat.lastMessage = message;
    }
  }
}