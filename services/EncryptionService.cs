using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class EncryptionService : IEncryptionService
{
    private readonly IEncryptionRepository _repository;
    private readonly FirebaseRepository _firebaseRepository;

    public EncryptionService(IEncryptionRepository repository, FirebaseRepository firebaseRepository)
    {
        _repository = repository;
        _firebaseRepository = firebaseRepository;
    }

    public string Encrypt(string plainText)
    {
        var encrypted = _repository.Encrypt(plainText);
        // Guarda en Firebase con un GUID como id
        _firebaseRepository.SaveEncryptedTextAsync(Guid.NewGuid().ToString(), encrypted).Wait();
        return encrypted;
    }

    public string Decrypt(string cipherText)
    {
        return _repository.Decrypt(cipherText);
    }

    public string GetEncryptedFromFirebase(string id)
    {
        return _firebaseRepository.GetEncryptedTextAsync(id).Result;
    }

    public (string Encrypted, string Id) EncryptWithId(string plainText)
    {
        var encrypted = _repository.Encrypt(plainText);
        var id = Guid.NewGuid().ToString();
        _firebaseRepository.SaveEncryptedTextAsync(id, encrypted).Wait();
        return (encrypted, id);
    }
} 