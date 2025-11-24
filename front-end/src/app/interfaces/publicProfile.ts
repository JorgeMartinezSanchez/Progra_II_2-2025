export interface PublicProfile {
    accountId: string
    username: string
    base64pfp: string
    publicKey: string          // Clave pública RSA (visible para todos)
}