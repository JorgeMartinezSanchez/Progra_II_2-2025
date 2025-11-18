import { Account } from "./account"
//import { GroupChat } from "./groupChat"
import { PrivateChat } from "./privateChat"
import { PublicProfile } from "./publicProfile"

export interface AppState {
    currentUser: Account
    contacts: Map<string, PublicProfile>  // userId -> perfil público
    privateChats: PrivateChat[]
    //groupChats: GroupChat[]
    
    // Claves descifradas en memoria (nunca persistir)
    sessionKeys?: {
        userPrivateKey: CryptoKey          // Clave privada RSA descifrada
        derivedPasswordKey: CryptoKey      // Clave derivada de contraseña
    }
}