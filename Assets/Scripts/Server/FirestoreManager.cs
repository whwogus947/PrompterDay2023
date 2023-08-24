using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Extensions;

public class FirestoreManager : MonoBehaviour
{
    [SerializeField] Button updateCountButton;
    [SerializeField] Text countUI;
    [SerializeField] InputField inputUI;
    FirebaseFirestore db;
    ListenerRegistration listenerRegistration;
    // Start is called before the first frame update
    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        updateCountButton.onClick.AddListener(OnHandleClick);
        listenerRegistration = db.Collection("User").Document("Counters").Listen(snapshot =>
        {
            Counters counter = snapshot.ConvertTo<Counters>();
            countUI.text = counter.Count.ToString();
        });


        // GetData();
    }
    void OnDestroy()
    {
        listenerRegistration.Stop();
    }
    void OnHandleClick()
    {
        int oldCount = int.Parse(inputUI.text);
        // Using Structs
        Counters counter = new Counters
        {
            Count = oldCount + 1,
            UserName = "HollyMolly"
        };

        // Using Dictionary
        // Dictionary<string, object> counter = new Dictionary<string, object>
        // {
        //     {"Count",oldCount+1},
        //     {"UpdatedBy","Vikings(Dictionary)"}
        // };
        DocumentReference countRef = db.Collection("User").Document("Counters");
        countRef.SetAsync(counter).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Updated Counter");
            // GetData();
        });

    }

    void GetData()
    {
        db.Collection("User").Document("Counters").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            Counters counter = task.Result.ConvertTo<Counters>();
            countUI.text = counter.Count.ToString();
        });
    }
}
