export interface Account {
    userId: number              // Identificador único del usuario
    username: string            // Nombre de usuario
    base64pfp: string          // Foto de perfil en base64
    publicKey: string          // Clave pública RSA (para recibir mensajes)
    encryptedPrivateKey: string // Clave privada RSA cifrada con contraseña del usuario
    salt: string               // Salt para derivar clave de contraseña (PBKDF2)
}