import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

console.log('🚀 Iniciando aplicación Angular...');

// Agrega un timeout para detectar bloqueos
const bootstrapPromise = bootstrapApplication(AppComponent, appConfig);

// Timeout de seguridad
const timeoutPromise = new Promise((_, reject) => {
  setTimeout(() => reject(new Error('Timeout: La aplicación tardó demasiado en iniciar')), 10000);
});

Promise.race([bootstrapPromise, timeoutPromise])
  .then(() => {
    console.log('✅ Aplicación Angular iniciada correctamente');
  })
  .catch((error) => {
    console.error('💥 ERROR CRÍTICO AL INICIAR:', error);
    
    // Mostrar error en pantalla
    const errorElement = document.createElement('div');
    errorElement.style.cssText = `
      position: fixed; top: 0; left: 0; width: 100%; 
      background: #ff4444; color: white; padding: 20px; 
      z-index: 9999; font-family: Arial; font-size: 14px;
      border-bottom: 2px solid #cc0000;
    `;
    errorElement.innerHTML = `
      <strong>Error al iniciar la aplicación:</strong><br>
      ${error.toString()}<br>
      <small>Revisa la consola del navegador para más detalles</small>
    `;
    document.body.appendChild(errorElement);
  });
