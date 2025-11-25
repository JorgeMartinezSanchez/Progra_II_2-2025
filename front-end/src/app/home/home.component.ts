import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs'; // 🔥 IMPORTANTE: Agrega esto

// Componentes
import { ChatComponent } from "../chat/chat.component";

// Servicios Reales
import { ChatService } from '../../services/chat/chat.service';
import { StateService } from '../../services/State/state.service';

// Interfaces Reales
import { ReceiveChat } from '../../app/interfaces/receiveChat';
import { RecieveAccount } from '../../app/interfaces/receiveAccount';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, ChatComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit, OnDestroy {
  
  // Estado del usuario actual (para mostrar tu foto y nombre en la esquina)
  public currentUser: RecieveAccount | null = null;
  
  // Lista de chats reales cargados del backend
  public chats: ReceiveChat[] = [];
  
  // Chat seleccionado para pasar al componente <app-chat>
  public selectedChat: ReceiveChat | null = null;

  public loading: boolean = true;

  // Inyección de dependencias
  private router = inject(Router);
  private chatService = inject(ChatService);
  private stateService = inject(StateService);

  private stateSubscription: Subscription | undefined;

  async ngOnInit() {
    console.log('🏠 HomeComponent inicializando...');
    
    // Suscribirse UNA VEZ al estado inicial
    this.stateSubscription = this.stateService.state$.subscribe(async (state) => {
      console.log('🔄 Estado actualizado:', state.currentUser?.username);
      
      this.currentUser = state.currentUser;

      // Solo redirigir si NO hay usuario Y no estamos ya en login
      if (!this.currentUser) {
        console.log('❌ No hay usuario, redirigiendo a login...');
        if (this.router.url !== '/login') {
          this.router.navigate(['/login']);
        }
        return;
      }

      // Solo cargar chats si hay usuario y no los tenemos ya
      if (this.currentUser && this.chats.length === 0 && !this.loading) {
        console.log('✅ Usuario válido, cargando chats...');
        await this.loadChats();
      }
    });
  }

  ngOnDestroy() {
    // 🔥 IMPORTANTE: Limpiar suscripción
    if (this.stateSubscription) {
      this.stateSubscription.unsubscribe();
      console.log('✅ Suscripción limpiada');
    }
  }

  async loadChats() {
    try {
      this.loading = true;
      console.log('📥 Cargando chats...');
      this.chats = await this.chatService.loadMyChats();
      console.log('✅ Chats cargados:', this.chats.length);
    } catch (error) {
      console.error('❌ Error cargando chats:', error);
    } finally {
      this.loading = false;
    }
  }

  // Seleccionar un chat de la lista
  selectChat(chat: ReceiveChat) {
    this.selectedChat = chat;
    // Opcional: Marcar como visto, etc.
  }

  // Navegación (igual que tu prototipo)
  move(opcion: string){
    this.router.navigate([opcion]);
  }

  // Helper para mostrar foto por defecto si no tienen
  getProfileImage(base64: string | undefined): string {
    return base64 || '/assets/default-user.png'; // Asegúrate de tener una imagen default
  }
}