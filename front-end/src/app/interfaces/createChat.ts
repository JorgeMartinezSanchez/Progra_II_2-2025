export interface CreateChat{
    accountId: string
    sendingUsername: string
    encryptedChatKeyForMe: string
    encryptedChatKeyForThem: string
}