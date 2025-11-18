import { Chat } from "./chat"
import { EncryptedMessage } from "./encryptedMessage"

export interface PrivateChat extends Chat {
    chatId: string             // ID único del chat
    user1Id: string            // ID del primer usuario
    user2Id: string            // ID del segundo usuario
    user1PublicKey: string     // Copia de clave pública user1 (por si cambia)
    user2PublicKey: string     // Copia de clave pública user2 (por si cambia)
    encryptedMessages: EncryptedMessage[]
    createdAt: Date
    lastMessageAt: Date
}