import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';
import { RecieveAccount } from '../interfaces/receiveAccount';

@Component({
  selector: 'app-user-config',
  imports: [],
  templateUrl: './user-config.component.html',
  styleUrl: './user-config.component.css'
})
export class UserConfigComponent {
  @Input() hostUser: RecieveAccount | null = null;
  @Output() chatOutput = new EventEmitter<RecieveAccount>();
  private router = inject(Router);
  private authService = inject(AuthService); // Inyectar el servicio
  public popOutWarning: boolean = false;

  back(): void{
    this.router.navigate(['/']);
  }

  logout(): void{
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
