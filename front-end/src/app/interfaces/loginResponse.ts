import { RecieveAccount } from "./receiveAccount";

export interface LoginResponse {
  token: string;
  accountId: string;
  username: string;
  base64Pfp: string;
  publicKey: string;
  encryptedPrivateKey: string;
  salt: string;
  createdAt: string;
  updatedAt: string;
}