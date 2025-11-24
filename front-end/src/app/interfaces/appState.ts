//import { GroupChat } from "./groupChat"
//import { PrivateChat } from "./privateChat"
import { PublicProfile } from "./publicProfile"
import { RecieveAccount } from "./receiveAccount"

export interface AppState {
    currentUser: RecieveAccount
    contacts: Map<string, PublicProfile>
    //privateChats: PrivateChat[]

    // Claves descifradas en memoria (nunca persistir)
    sessionKeys?: {
        userPrivateKey: CryptoKey          // Clave privada RSA descifrada
        derivedPasswordKey?: CryptoKey      // Clave derivada de contraseña
    }
}