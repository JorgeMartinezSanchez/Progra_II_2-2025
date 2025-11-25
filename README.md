<<<<<<< HEAD
# FrontEnd

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 19.2.9.
=======
# SafeChat - Backend de Aplicación de Mensajería Cifrada End-to-End

**Carrera:** Ingeniería de Software  
**Materia:** Programación II  
**Periodo:** Segundo Parcial / Proyecto Final  
**Estudiante:** Jorge Martínez Sánchez  
**Fecha de entrega:** 2025-11-24 (Tentativa)
>>>>>>> 531df5df4f3d57a87d5f002d7ffa431f0ba70e65

## Development server

<<<<<<< HEAD
To start a local development server, run:

```bash
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.
=======
## 📋 Datos Generales del Proyecto

| Campo | Descripción |
|--------|-------------|
| **Nombre del proyecto** | SafeChat |
| **Tipo de aplicación** | ✅ API Backend |
| **Tecnologías principales** | C# ASP.NET Core Web API |
| **Base de datos** | MongoDB |
| **Repositorio Git** | [github.com/JorgeMartinezSanchez/Progra_II_2-2025](https://github.com/JorgeMartinezSanchez/Progra_II_2-2025) |
| **Uso de IA** | ✅ Sí (Claude AI / DeepSeek para guía técnica y arquitectura) |
>>>>>>> 531df5df4f3d57a87d5f002d7ffa431f0ba70e65

## Code scaffolding

<<<<<<< HEAD
Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```
=======
## 🎯 Descripción del Proyecto

SafeChat es el backend de una aplicación de mensajería instantánea que implementa **cifrado end-to-end (E2EE)** utilizando una combinación de **cifrado simétrico AES** y **cifrado asimétrico RSA**, garantizando que solo los participantes de una conversación puedan leer los mensajes.

### Objetivos Principales

1. **Seguridad**: Implementar cifrado end-to-end usando RSA-2048 y AES-256
2. **Privacidad**: El servidor nunca tiene acceso a las claves privadas ni al contenido de los mensajes
3. **Arquitectura robusta**: Aplicar principios SOLID y patrones de diseño (Repository, Service Layer)
4. **API RESTful**: Endpoints bien definidos para clientes seguros
>>>>>>> 531df5df4f3d57a87d5f002d7ffa431f0ba70e65

## Building

<<<<<<< HEAD
To build the project run:

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Karma](https://karma-runner.github.io) test runner, use the following command:
=======
## 🔐 Arquitectura de Seguridad

### Flujo de Cifrado

```
┌─────────────────────────────────────────────────────────────┐
│ 1. REGISTRO DE USUARIO                                      │
│    • Cliente genera par de claves RSA (2048 bits)          │
│    • Clave privada → cifrada con contraseña (PBKDF2)       │
│    • Clave pública → enviada al servidor                    │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│ 2. CREACIÓN DE CHAT                                         │
│    • Cliente A genera clave AES-256 aleatoria              │
│    • Cifra AES con RSA pública de Usuario A → KeyStore A   │
│    • Cifra AES con RSA pública de Usuario B → KeyStore B   │
│    • Ambas claves cifradas se guardan en ChatKeyStore      │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│ 3. ENVÍO DE MENSAJE                                         │
│    • Cliente obtiene EncryptedChatKey del servidor         │
│    • Descifra con su clave privada RSA → obtiene AES       │
│    • Cifra mensaje con AES-256 (modo CBC)                  │
│    • Envía mensaje cifrado + IV al servidor                 │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│ 4. RECEPCIÓN DE MENSAJE                                     │
│    • Cliente B recibe mensaje cifrado + IV                  │
│    • Obtiene su EncryptedChatKey del servidor              │
│    • Descifra con su clave privada RSA → obtiene AES       │
│    • Descifra mensaje con AES + IV                         │
└─────────────────────────────────────────────────────────────┘
```

### Capas de Protección

1. **Capa 1**: Mensajes cifrados con AES-256 (clave única por chat)
2. **Capa 2**: Claves AES cifradas con RSA-2048 (clave única por usuario)
3. **Capa 3**: Claves privadas RSA cifradas con contraseña del usuario (PBKDF2 + Salt único)

**Resultado**: El servidor solo almacena datos cifrados que no puede descifrar.
>>>>>>> 531df5df4f3d57a87d5f002d7ffa431f0ba70e65

```bash
ng test
```

<<<<<<< HEAD
## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```
=======
## 🏗️ Diseño Técnico y Aplicación de POO

### Principios de POO Aplicados

- [x] **Encapsulamiento**: Repositorios y servicios con responsabilidades bien definidas
- [x] **Uso de constructores**: Inyección de dependencias en todos los servicios
- [x] **Herencia**: Clases base para entidades y servicios comunes
- [x] **Polimorfismo**: Interfaces `IAccountService`, `IMessageService`, etc.
- [x] **Interfaces**: Separación entre contratos (interfaces) e implementaciones
- [x] **Inyección de Dependencias**: ASP.NET Core DI Container

### Arquitectura Backend (C# ASP.NET Core)

```
┌──────────────────────────────────────────────────────────┐
│                      CONTROLLERS                         │
│  AccountController │ MessageController │ PrivateChatController
└──────────────────────────────────────────────────────────┘
                         ↓
┌──────────────────────────────────────────────────────────┐
│                       SERVICES                           │
│  AccountService │ MessageService │ PrivateChatService    │
│                 ChatKeyStoreService │ DesencrypteService │
└──────────────────────────────────────────────────────────┘
                         ↓
┌──────────────────────────────────────────────────────────┐
│                     REPOSITORIES                         │
│  AccountRepository │ MessageRepository                   │
│  PrivateChatRepository │ ChatKeyStoreRepository          │
└──────────────────────────────────────────────────────────┘
                         ↓
┌──────────────────────────────────────────────────────────┐
│                    MONGODB DATABASE                      │
│  Account │ Message │ PrivateChat │ ChatKeyStore         │
└──────────────────────────────────────────────────────────┘
```

### Clases Principales del Backend

| Clase | Responsabilidad |
|-------|----------------|
| `Account` | Datos del usuario (username, claves RSA, salt) |
| `Message` | Mensaje cifrado con AES + IV |
| `PrivateChat` | Relación entre dos usuarios |
| `ChatKeyStore` | Clave AES del chat cifrada con RSA por usuario |
| `AccountService` | Lógica de negocio para usuarios |
| `MessageService` | Lógica de envío/recepción de mensajes |
| `PrivateChatService` | Gestión de chats y contactos |
| `ChatKeyStoreService` | Distribución segura de claves |
| `DesencrypteService` | Servicios de descifrado AES y RSA |

### Servicios de Descifrado

El `DesencrypteService` proporciona múltiples métodos de descifrado:

- **`DesencryptePassword`**: Descifrado PBKDF2 + AES para contraseñas
- **`DesencrypteWithAES`**: Descifrado AES directo con clave e IV
- **`DesencrypteWithRSA`**: Descifrado RSA con clave privada
- **`DesencrypteChatMessage`**: Especializado para mensajes de chat
- **`DesencrypteChatKey`**: Especializado para claves de chat

### Persistencia de Datos

- [x] **Base de datos**: MongoDB
- **Colecciones**:
  - `Account`: Información de usuarios y claves públicas
  - `Message`: Mensajes cifrados con metadata
  - `PrivateChat`: Relaciones de chat entre usuarios
  - `ChatKeyStore`: Claves cifradas de chat (2 registros por chat)
>>>>>>> 531df5df4f3d57a87d5f002d7ffa431f0ba70e65

Angular CLI does not come with an end-to-end testing framework by default. You can choose one that suits your needs.

<<<<<<< HEAD
## Additional Resources

For more information on using the Angular CLI, including detailed command references, visit the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
=======
## ⚙️ Funcionalidades Implementadas

| Nº | Funcionalidad | Descripción | Estado |
|----|---------------|-------------|--------|
| 1 | Registro de usuarios | Generación de par RSA, cifrado de clave privada, almacenamiento seguro | ✅ Implementado |
| 2 | Creación de chats privados | Intercambio seguro de claves AES cifradas con RSA | ✅ Implementado |
| 3 | Envío de mensajes cifrados | Cifrado AES-256 de mensajes con IV único | ✅ Implementado |
| 4 | Recepción y descifrado | Obtención de claves y descifrado en cliente | ✅ Implementado |
| 5 | Gestión de contactos | Listar chats activos con información de contactos | ✅ Implementado |
| 6 | Eliminación de chats | Borrado en cascada (mensajes + claves + chat) | ✅ Implementado |
| 7 | Marcar mensajes como vistos | Actualización de estado de mensajes | ✅ Implementado |
| 8 | API optimizada con DTOs | Reducción de llamadas HTTP (1 request vs N+1) | ✅ Implementado |

### 🚧 Funcionalidades Pendientes

- [ ] Autenticación con JWT tokens
- [ ] Notificaciones en tiempo real (SignalR/WebSockets)
- [ ] Recuperación de cuenta
- [ ] Múltiples dispositivos por usuario

---

## 🛠️ Tecnologías Utilizadas

### Backend
- **Framework**: ASP.NET Core 9.0 Web API
- **Lenguaje**: C# 12
- **Base de datos**: MongoDB 7.x
- **Driver**: MongoDB.Driver
- **Patrón**: Repository + Service Layer
- **Inyección de dependencias**: Built-in ASP.NET Core DI
- **Cifrado**: System.Security.Cryptography (AES, RSA, PBKDF2)

### Herramientas de Desarrollo
- **IDE**: Visual Studio 2022 / VS Code
- **API Testing**: Swagger UI / Postman
- **Control de versiones**: Git + GitHub
- **IA Asistente**: Claude AI (Anthropic) para arquitectura y debugging

---

## 📦 Estructura del Proyecto Backend

```
back-end/
│
├── Controllers/             # Endpoints REST
│   ├── AccountController.cs
│   ├── MessageController.cs
│   ├── PrivateChatController.cs
│   └── ChatKeyStoreController.cs
│
├── Services/                # Lógica de negocio
│   ├── AccountService.cs
│   ├── MessageService.cs
│   ├── PrivateChatService.cs
│   ├── ChatKeyStoreService.cs
│   └── DesencrypteService.cs
│
├── Repository/              # Acceso a datos
│   ├── AccountRepository.cs
│   ├── MessageRepository.cs
│   ├── PrivateChatRepository.cs
│   └── ChatKeyStoreRepository.cs
│
├── Models/                  # Entidades MongoDB
│   ├── Account.cs
│   ├── Message.cs
│   ├── PrivateChat.cs
│   └── ChatKeyStore.cs
│
├── DTOs/                    # Data Transfer Objects
│   ├── CreateAccountDto.cs
│   ├── ReceiveAccountDto.cs
│   ├── CreateMessageDto.cs
│   ├── ReceiveMessageDto.cs
│   ├── CreatePrivateChatDto.cs
│   ├── ReceivePrivateChatDto.cs
│   ├── CreateChatKeyStoreDto.cs
│   ├── ReceiveChatKeyStoreDto.cs
│   └── LoginDto.cs
│
├── Interfaces/              # Contratos de servicios
└── Program.cs               # Configuración y DI
```

---

## 🚀 Instalación y Ejecución

### Prerrequisitos
- .NET SDK 9.0+
- MongoDB 7.x (local o Atlas)

### Backend (API)

```bash
cd back-end

# Configurar MongoDB en appsettings.json
# {
#   "MongoDB": {
#     "ConnectionString": "mongodb://localhost:27017",
#     "DatabaseName": "SafeChatDB"
#   }
# }

# Restaurar dependencias
dotnet restore

# Ejecutar
dotnet run
# API disponible en: https://localhost:5053
# Swagger UI en: https://localhost:5053/swagger
```

---

## 🧪 Testing con Swagger

1. Navega a `https://localhost:5053/swagger`
2. **Crear cuenta**:
   ```json
   POST /api/Account
   {
     "username": "usuario_test",
     "base64Pfp": "",
     "publicKey": "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA...",
     "encryptedPrivateKey": "ENCRYPTED_PRIVATE_KEY_HERE",
     "salt": "RANDOM_SALT_HERE"
   }
   ```
3. **Crear chat privado**:
   ```json
   POST /api/PrivateChat
   {
     "accountId": "67a3b2c1d4e5f6a7b8c9d0e1",
     "sendingUsername": "otro_usuario",
     "encryptedChatKeyForMe": "ENCRYPTED_AES_KEY_FOR_ME",
     "encryptedChatKeyForThem": "ENCRYPTED_AES_KEY_FOR_THEM"
   }
   ```
4. **Enviar mensaje**:
   ```json
   POST /api/Message
   {
     "chatId": "67a3b2c1d4e5f6a7b8c9d0e2",
     "senderId": "67a3b2c1d4e5f6a7b8c9d0e1",
     "encryptedContent": "U2FsdGVkX1...",
     "iv": "1234567890abcdef"
   }
   ```

---

## 🔒 Consideraciones de Seguridad

### Implementado ✅
- Cifrado end-to-end con RSA-2048 + AES-256
- Claves privadas nunca enviadas al servidor
- Salt único por usuario para derivación de claves
- IV único por mensaje
- Arquitectura "Zero Knowledge" del servidor
- Validación de datos de entrada con Data Annotations

### Por Implementar 🚧
- HTTPS obligatorio en producción
- Rate limiting en endpoints
- Autenticación con JWT
- Validación de firma digital de mensajes
- Perfect Forward Secrecy (rotación de claves)

---

## 📚 Patrones y Buenas Prácticas

- **Repository Pattern**: Abstracción de acceso a datos
- **Service Layer**: Lógica de negocio separada de controllers
- **Dependency Injection**: Acoplamiento débil entre componentes
- **DTOs**: Separación entre modelos de dominio y API
- **Async/Await**: Operaciones I/O no bloqueantes
- **Error Handling**: Try-catch con respuestas HTTP apropiadas
- **SOLID Principles**: Single Responsibility, Open/Closed, etc.
- **Validation**: Data Annotations en DTOs

---

## 👤 Autor

**Jorge Martínez Sánchez**  
Ingeniería de Software  
Universidad: *Universidad Catolica Boliviana San Pablo*

---

## 📄 Licencia

Este proyecto es un trabajo académico para la materia de Programación II.

---

## 🙏 Agradecimientos

- Claude AI (Anthropic) por asistencia técnica en arquitectura de seguridad
- Documentación oficial de ASP.NET Core y MongoDB
- Comunidad de Stack Overflow

---

**Última actualización:** Noviembre 2024  
**Estado del proyecto:** 🟢 Backend 90% completo (API funcional y segura)
>>>>>>> 531df5df4f3d57a87d5f002d7ffa431f0ba70e65
