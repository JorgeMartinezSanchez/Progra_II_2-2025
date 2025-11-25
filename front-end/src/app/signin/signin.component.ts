import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../services/account/account.service';
import { Base64imageParserService } from '../../services/base64imageParser/base64image-parser.service';

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
  public isCreatingAccount: boolean = false; // ✅ NUEVO
  
  public selectedProfilePicBase64: string = ''; 
  public previewUrl: string = '/default-pics/keesy.jpg'; 

  public profilePictures: string[] = [
    '/default-pics/keesy.jpg',
    '/default-pics/lock.png', 
    '/default-pics/snake thief.jpg'
  ];

  constructor(
    private router: Router, 
    private accService: AccountService,
    private imageService: Base64imageParserService
  ) {
    this.selectProfilePicture(this.profilePictures[0]);
  }

  async selectProfilePicture(picPath: string) {
    this.previewUrl = picPath;
    try {
      this.selectedProfilePicBase64 = await this.urlToBase64(picPath);
      console.log('✅ Imagen predefinida convertida a Base64');
    } catch (error) {
      console.error('❌ Error procesando imagen predefinida', error);
    }
  }

  async onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      try {
        const base64 = await this.imageService.fileToBase64(file);
        this.selectedProfilePicBase64 = base64;
        this.previewUrl = base64;
        this.editPfp = false;
        console.log('✅ Imagen propia cargada');
      } catch (error) {
        console.error('❌ Error al leer el archivo', error);
      }
    }
  }

  selectRandomPicture(): void {
    const randomIndex = Math.floor(Math.random() * this.profilePictures.length);
    this.selectProfilePicture(this.profilePictures[randomIndex]);
  }

  confirmProfilePicture(): void {
    this.editPfp = false;
  }

  createAccount(): void {
    // ✅ Validaciones mejoradas
    if (!this.username || !this.password) {
      alert('⚠️ Por favor completa todos los campos');
      return;
    }

    if (this.username.length < 3 || this.username.length > 20) {
      alert('⚠️ El nombre de usuario debe tener entre 3 y 20 caracteres');
      return;
    }

    if (!/^[a-zA-Z0-9_]+$/.test(this.username)) {
      alert('⚠️ El nombre de usuario solo puede contener letras, números y guiones bajos');
      return;
    }

    if (this.password.length < 6) {
      alert('⚠️ La contraseña debe tener al menos 6 caracteres');
      return;
    }

    if (!this.selectedProfilePicBase64) {
      this.selectRandomPicture();
    }

    // ✅ Mostrar estado de carga
    this.isCreatingAccount = true;

    console.log('🔐 Iniciando registro...');
    console.log('Username:', this.username);
    console.log('Password length:', this.password.length);
    console.log('Base64Pfp length:', this.selectedProfilePicBase64.length);

    this.accService.registerUser(
      this.username, 
      this.password, 
      this.selectedProfilePicBase64
    ).subscribe({
      next: (response) => {
        console.log('✅ Usuario creado exitosamente:', response);
        alert('✅ Cuenta creada con éxito. Ahora puedes iniciar sesión.');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('❌ Error creando cuenta:', err);
        alert('❌ Error al crear cuenta: ' + err.message);
      },
      complete: () => {
        this.isCreatingAccount = false; // ✅ Ocultar loading
      }
    });
  }

  handleImageError(event: any): void {
    console.error('Error loading image:', event.target.src);
    event.target.src = '/assets/placeholder.png';
  }

  private async urlToBase64(url: string): Promise<string> {
    try {
      const response = await fetch(url);
      const blob = await response.blob();
      return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onloadend = () => resolve(reader.result as string);
        reader.onerror = reject;
        reader.readAsDataURL(blob);
      });
    } catch (error) {
      console.error('Error converting URL to Base64:', error);
      // Fallback: imagen vacía 1x1 transparente
      return 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==';
    }
  }
}