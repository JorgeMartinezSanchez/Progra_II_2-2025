import { RecieveAccount } from "./receiveAccount";

export interface LoginResponse extends RecieveAccount {
    token: string
    encryptedPrivateKey: string
    salt: string
}