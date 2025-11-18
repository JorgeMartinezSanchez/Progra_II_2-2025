import { Component, inject} from '@angular/core';
import { ChatTest } from '../interfaces/testing_interfaces/chatTest';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ChatComponent } from "../chat/chat.component";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, ChatComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  public profilePicture: boolean = false;
  public chats: ChatTest[] = [];
  public selectedChat: ChatTest | null = null;
  private chatNamesId: string[] = [];
  private router = inject(Router);

  // Mapeo de userId a nombres reales - PARAMETRIZABLE
  public userMap: { [key: number]: string } = {
    1: 'Jaime',      // Tú
    2: 'Ana García',
    3: 'Carlos López',
    4: 'María Rodríguez',
    5: 'Pedro Martínez',
    6: 'Laura Sánchez'
  };

  // Método para seleccionar un chat
  selectChat(chat: ChatTest) {
    this.selectedChat = chat;
    console.log('Chat seleccionado:', chat.name);
  }

  move(opcion: string){
    this.router.navigate([opcion]);
  }

  // Método para obtener el nombre de un usuario por su ID
  getUserName(userId: number): string {
    return this.userMap[userId] || `Usuario ${userId}`;
  }

  // Método para agregar un nuevo usuario al mapeo
  addUserToMap(userId: number, userName: string) {
    this.userMap[userId] = userName;
  }

  ngOnInit() {
    // Datos de ejemplo con userIds que corresponden al userMap
    this.chats = [
      {
        chatId: '1',
        name: 'Ana García',
        lastMessage: 'Hola, ¿cómo estás?',
        isEncrypted: true,
        type: 'private',
        emisorUser: 'jaime',
        receptorUser: 'ana',
        messages: [
          {
            userId: 2, // Ana García
            message: 'Hola, ¿cómo estás?',
            sendDate: new Date()
          },
          {
            userId: 1, // Jaime (tú)
            message: '¡Hola Ana! Todo bien, ¿y tú?',
            sendDate: new Date()
          }
        ]
      },
      {
        chatId: '2',
        name: 'Grupo Familia',
        lastMessage: 'María: Feliz cumpleaños!',
        isEncrypted: true,
        type: 'group',
        members: ['Jaime', 'Ana García', 'Carlos López', 'María Rodríguez'],
        messages: [
          {
            userId: 4, // María Rodríguez
            message: '¡Feliz cumpleaños Jaime! 🎉',
            sendDate: new Date(Date.now() - 3600000) // 1 hora atrás
          },
          {
            userId: 3, // Carlos López
            message: 'Felicidades hermano 🥳',
            sendDate: new Date(Date.now() - 1800000) // 30 minutos atrás
          },
          {
            userId: 2, // Ana García
            message: '¡Muchas felicidades! ¿Planes para celebrar?',
            sendDate: new Date(Date.now() - 900000) // 15 minutos atrás
          },
          {
            userId: 1, // Jaime (tú)
            message: '¡Gracias a todos! Los espero en mi casa a las 8pm 🎂',
            sendDate: new Date()
          }
        ]
      },
      {
        chatId: '3',
        name: 'Equipo Trabajo',
        lastMessage: 'Pedro: Reunión a las 3pm',
        isEncrypted: true,
        type: 'group',
        members: ['Jaime', 'Pedro Martínez', 'Laura Sánchez'],
        messages: [
          {
            userId: 5, // Pedro Martínez
            message: 'Recordatorio: reunión de equipo hoy a las 3pm',
            sendDate: new Date(Date.now() - 7200000) // 2 horas atrás
          },
          {
            userId: 6, // Laura Sánchez
            message: 'Llevo el reporte de ventas',
            sendDate: new Date(Date.now() - 3600000) // 1 hora atrás
          }
        ]
      }
    ];
  }
}