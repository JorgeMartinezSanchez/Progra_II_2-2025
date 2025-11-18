export interface PublicProfile {
    userId: string
    username: string
    base64pfp: string
    publicKey: string          // Clave pública RSA (visible para todos)
}