import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { Account } from '../interfaces/account';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-user-config',
  imports: [],
  templateUrl: './user-config.component.html',
  styleUrl: './user-config.component.css'
})
export class UserConfigComponent {
  @Input() hostUser: Account | null = null;
  @Output() chatOutput = new EventEmitter<Account>();
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
