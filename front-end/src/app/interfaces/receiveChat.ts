export interface ReceiveChat{
    chatId: string
    accountId1: string
    accountId2: string
    createdAt: Date
    lastActivity: Date
    contactId?: string
    contactUsername?: string
    contactBase64Pfp?: string
}