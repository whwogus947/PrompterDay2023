using Firebase.Firestore;

[FirestoreData]
public struct Counters
{
    //Firestore Property to store count
    [FirestoreProperty]
    public int Count { get; set; }
    //Firestore Property to update UpdatedBy User
    [FirestoreProperty]
    public string UserName { get; set; }
}
