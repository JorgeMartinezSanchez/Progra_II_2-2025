import { EncryptedPayload } from "./encryptedPayLoad"

export interface SignedMessage {
    payload: EncryptedPayload
    signature: string
}