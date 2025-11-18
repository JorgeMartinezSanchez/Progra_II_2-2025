import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-signin',
  imports: [CommonModule, FormsModule],
  templateUrl: './signin.component.html',
  styleUrl: './signin.component.css'
})
export class SigninComponent {
  public editPfp: boolean = false;
  public username: string = '';
  public password: string = '';
  public selectedProfilePic: string = '';
  
  // Rutas corregidas - desde la raíz del proyecto
  public profilePictures: string[] = [
    '/default-pics/keesy.jpg',
    '/default-pics/lock.png', 
    '/default-pics/snake thief.jpg'
  ];

  constructor(private router: Router) {}

  // Seleccionar una foto específica
  selectProfilePicture(pic: string): void {
    this.selectedProfilePic = pic;
  }

  // Seleccionar una foto aleatoria
  selectRandomPicture(): void {
    const randomIndex = Math.floor(Math.random() * this.profilePictures.length);
    this.selectedProfilePic = this.profilePictures[randomIndex];
  }

  // Confirmar la selección de foto
  confirmProfilePicture(): void {
    this.editPfp = false;
    console.log('Foto de perfil seleccionada:', this.selectedProfilePic);
  }

  // Crear cuenta
  createAccount(): void {
    if (!this.username || !this.password) {
      alert('Please fill in all fields');
      return;
    }

    if (!this.selectedProfilePic) {
      this.selectRandomPicture();
    }

    console.log('Creating account with:', {
      username: this.username,
      password: this.password,
      profilePic: this.selectedProfilePic
    });

    // Simular creación de cuenta
    setTimeout(() => {
      alert('Account created successfully!');
      this.router.navigate(['/login']);
    }, 1000);
  }

  // Agregar este método a la clase SigninComponent
  handleImageError(event: any): void {
    console.error('Error loading image:', event.target.src);
    event.target.style.display = 'none';
    // Puedes mostrar un placeholder o manejar el error de otra forma
  }
}
