export interface EncryptedMessage {
    messageId: string          // ID único del mensaje
    senderId: string           // ID del remitente
    encryptedContent: string   // Contenido cifrado con AES-GCM
    encryptedAESKey: string    // Clave AES cifrada con RSA del destinatario
    iv: string                 // Vector de inicialización para AES
    authTag: string            // Tag de autenticación de AES-GCM
    signature: string          // Firma RSA del mensaje (verifica identidad)
    timestamp: Date            // Momento del envío
    sent: boolean              // Estado de envío
    read: boolean              // Estado de lectura
}