import { Chat } from "./chat"
import { EncryptedMessage } from "./encryptedMessage"

export interface GroupChat extends Chat {
    chatId: string             // ID único del grupo
    groupName: string          // Nombre del grupo
    groupIcon: string         // Icono del grupo (opcional)
    adminId: string            // ID del administrador
    memberIds: string[]        // IDs de todos los miembros
    
    // Clave AES del grupo cifrada con la clave pública RSA de cada miembro
    memberEncryptedKeys: Map<string, string> // userId -> encryptedGroupAESKey
    
    // Versión de la clave (incrementa si se rota la clave del grupo)
    keyVersion: number
    
    encryptedMessages: EncryptedMessage[]
    createdAt: Date
    lastMessageAt: Date
}