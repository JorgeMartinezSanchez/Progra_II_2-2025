export interface EncryptedPayload {
    encryptedContent: string
    encryptedAESKey: string
    iv: string
    authTag: string
}