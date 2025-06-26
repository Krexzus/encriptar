public interface IEncryptionRepository
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
} 