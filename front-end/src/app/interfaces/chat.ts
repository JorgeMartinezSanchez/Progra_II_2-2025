import { EncryptedMessage } from "./encryptedMessage"

export interface Chat{
    chatId: string
    encryptedMessages: EncryptedMessage[]
    creationDate: Date
}