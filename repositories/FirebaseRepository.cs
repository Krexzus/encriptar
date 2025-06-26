using Google.Cloud.Firestore;
using System.Threading.Tasks;

public class FirebaseRepository
{
    private readonly FirestoreDb _firestoreDb;

    public FirebaseRepository(string credentialsPath, string projectId)
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
        _firestoreDb = FirestoreDb.Create(projectId);
    }

    public async Task SaveEncryptedTextAsync(string id, string encryptedText)
    {
        DocumentReference docRef = _firestoreDb.Collection("encrypted_texts").Document(id);
        await docRef.SetAsync(new { EncryptedText = encryptedText });
    }

    public async Task<string> GetEncryptedTextAsync(string id)
    {
        DocumentReference docRef = _firestoreDb.Collection("encrypted_texts").Document(id);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists && snapshot.ContainsField("EncryptedText"))
        {
            return snapshot.GetValue<string>("EncryptedText");
        }
        return null;
    }
} 