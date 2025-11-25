export interface ReceiveMessage{
    messageId: string
    chatId: string
    senderId: string
    encryptedContent: string
    content?: string
    iv: string
    timeStamp: string
    status: string
}