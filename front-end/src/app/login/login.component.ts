// login.component.ts
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  credentials = {
    username: '',
    password: ''
  };
  isLoading = false;
  errorMessage = '';

  private authService = inject(AuthService);
  private router = inject(Router);

  onSubmit() {
    if (!this.credentials.username || !this.credentials.password) {
      this.errorMessage = 'Please enter both username and password';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    // Simular login (reemplaza con tu lógica real)
    this.simulateLogin();
  }

  private simulateLogin() {
    // Simular una llamada a la API
    setTimeout(() => {
      // Aquí deberías usar this.authService.login() con tu backend real
      if (this.credentials.username && this.credentials.password) {
        // Guardar token simulado
        localStorage.setItem('authToken', 'simulated-token-' + Date.now());
        localStorage.setItem('currentUser', this.credentials.username);
        
        // Redirigir al home
        this.router.navigate(['/']);
      } else {
        this.errorMessage = 'Invalid credentials';
      }
      this.isLoading = false;
    }, 1500);
  }

  signUp() {
    this.router.navigate(['/signin']);
  }

  ngOnInit() {
    // Si ya está logueado, redirigir al home
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/']);
    }
  }
}
