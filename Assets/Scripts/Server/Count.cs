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

[FirestoreData]
public struct textDatas
{
    [FirestoreProperty]
    public string UserName { get; set; }
    [FirestoreProperty]
    public string Text { get; set; }
    [FirestoreProperty]
    public string Time { get; set; }
}