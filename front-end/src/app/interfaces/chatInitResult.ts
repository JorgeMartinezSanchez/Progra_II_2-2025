export interface ChatInitResult {
  rawAesKey: CryptoKey;
  encryptedForMe: string;
  encryptedForThem: string;
}