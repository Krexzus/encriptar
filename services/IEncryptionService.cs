public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    (string Encrypted, string Id) EncryptWithId(string plainText);
    string GetEncryptedFromFirebase(string id);
} 