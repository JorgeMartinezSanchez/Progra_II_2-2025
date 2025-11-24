# SafeChat - Aplicación de Mensajería Cifrada End-to-End

**Carrera:** Ingeniería de Software  
**Materia:** Programación II  
**Periodo:** Segundo Parcial / Proyecto Final  
**Estudiante:** Jorge Martínez Sánchez  
**Fecha de entrega:** 2025-12-02 (Tentativa)

---

## 📋 Datos Generales del Proyecto

| Campo | Descripción |
|--------|-------------|
| **Nombre del proyecto** | SafeChat |
| **Tipo de aplicación** | ✅ Aplicación Web |
| **Tecnologías principales** | C# ASP.NET Core Web API + Angular + TypeScript |
| **Base de datos** | MongoDB |
| **Repositorio Git** | [github.com/JorgeMartinezSanchez/Progra_II_2-2025](https://github.com/JorgeMartinezSanchez/Progra_II_2-2025) |
| **Uso de IA** | ✅ Sí (Claude AI / DeepSeek para guía técnica y arquitectura) |

---

## 🎯 Descripción del Proyecto

SafeChat es una aplicación de mensajería instantánea que implementa **cifrado end-to-end (E2EE)** utilizando una combinación de **cifrado simétrico AES** y **cifrado asimétrico RSA**, garantizando que solo los participantes de una conversación puedan leer los mensajes.

### Objetivos Principales

1. **Seguridad**: Implementar cifrado end-to-end usando RSA-2048 y AES-256
2. **Privacidad**: El servidor nunca tiene acceso a las claves privadas ni al contenido de los mensajes
3. **Arquitectura robusta**: Aplicar principios SOLID y patrones de diseño (Repository, Service Layer)
4. **Escalabilidad**: Diseño preparado para soportar múltiples chats simultáneos por usuario

---

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

---

## 🏗️ Diseño Técnico y Aplicación de POO

### Principios de POO Aplicados

- [x] **Encapsulamiento**: Repositorios y servicios con responsabilidades bien definidas
- [x] **Uso de constructores**: Inyección de dependencias en todos los servicios
- [x] **Herencia**: Clase base `APIdataReciever` para servicios del frontend
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
│                 ChatKeyStoreService                      │
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

### Clases Principales

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

### Persistencia de Datos

- [x] **Base de datos**: MongoDB
- **Colecciones**:
  - `Account`: Información de usuarios y claves públicas
  - `Message`: Mensajes cifrados con metadata
  - `PrivateChat`: Relaciones de chat entre usuarios
  - `ChatKeyStore`: Claves cifradas de chat (2 registros por chat)

---

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

- [ ] Interfaz de usuario completa (Angular)
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

### Frontend
- **Framework**: Angular 18+
- **Lenguaje**: TypeScript 5.x
- **HTTP Client**: RxJS + HttpClient
- **Cifrado**: Web Crypto API (SubtleCrypto)

### Herramientas de Desarrollo
- **IDE Backend**: Visual Studio 2022 / VS Code
- **IDE Frontend**: VS Code
- **API Testing**: Swagger UI / Postman
- **Control de versiones**: Git + GitHub
- **IA Asistente**: Claude AI (Anthropic) para arquitectura y debugging

---

## 📦 Estructura del Proyecto

```
Progra_II_2-2025/
│
├── back-end/                    # API en C#
│   ├── Controllers/             # Endpoints REST
│   │   ├── AccountController.cs
│   │   ├── MessageController.cs
│   │   ├── PrivateChatController.cs
│   │   └── ChatKeyStoreController.cs
│   │
│   ├── Services/                # Lógica de negocio
│   │   ├── AccountService.cs
│   │   ├── MessageService.cs
│   │   ├── PrivateChatService.cs
│   │   └── ChatKeyStoreService.cs
│   │
│   ├── Repository/              # Acceso a datos
│   │   ├── AccountRepository.cs
│   │   ├── MessageRepository.cs
│   │   ├── PrivateChatRepository.cs
│   │   └── ChatKeyStoreRepository.cs
│   │
│   ├── Models/                  # Entidades MongoDB
│   │   ├── Account.cs
│   │   ├── Message.cs
│   │   ├── PrivateChat.cs
│   │   └── ChatKeyStore.cs
│   │
│   ├── DTOs/                    # Data Transfer Objects
│   │   ├── CreateAccountDto.cs
│   │   ├── ReceiveAccountDto.cs
│   │   ├── CreateMessageDto.cs
│   │   ├── ReceiveMessageDto.cs
│   │   └── ...
│   │
│   ├── Interfaces/              # Contratos de servicios
│   └── Program.cs               # Configuración y DI
│
└── front-end/                   # Cliente Angular
    ├── src/
    │   ├── app/
    │   │   ├── services/        # Servicios HTTP
    │   │   ├── interfaces/      # Tipos TypeScript
    │   │   └── components/      # Componentes UI
    │   └── ...
    └── ...
```

---

## 🚀 Instalación y Ejecución

### Prerrequisitos
- .NET SDK 9.0+
- Node.js 18+ y npm
- MongoDB 7.x (local o Atlas)
- Angular CLI (`npm install -g @angular/cli`)

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

### Frontend (Angular)

```bash
cd front-end

# Instalar dependencias
npm install

# Ejecutar en desarrollo
ng serve
# App disponible en: http://localhost:4200
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

---

## 👤 Autor

**Jorge Martínez Sánchez**  
Ingeniería de Software  
Universidad: *[Tu Universidad]*

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
**Estado del proyecto:** 🟡 En desarrollo activo (Backend 90% completo, Frontend 30%)
