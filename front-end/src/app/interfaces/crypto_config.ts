export const CRYPTO_CONFIG = {
    RSA: {
        modulusLength: 2048,
        publicExponent: new Uint8Array([1, 0, 1]),
        hash: 'SHA-256'
    },
    AES: {
        algorithm: 'AES-GCM',
        keyLength: 256,
        ivLength: 12,
        tagLength: 128
    },
    PBKDF2: {
        iterations: 100000,
        hash: 'SHA-256',
        saltLength: 16
    }
} as const;