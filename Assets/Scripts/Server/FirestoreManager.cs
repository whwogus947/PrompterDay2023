using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
    int num = 0;
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

        string name = "yejin";
        string text = "안녕하세요";
        System.DateTime localDate = System.DateTime.Now;
        string formattedTime = localDate.ToString("yyyy.MM.dd HH:mm:ss");

        sendTextToServer(name, text, formattedTime);

    }

    void GetData()
    {
        db.Collection("User").Document("Counters").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            Counters counter = task.Result.ConvertTo<Counters>();
            countUI.text = counter.Count.ToString();
        });
    }

    void sendTextToServer(string userName, string text, string time)
    {
        textDatas textData = new textDatas
        {
            UserName = userName,
            Text = text,
            Time = time
        };

        num += 1;
        string partName = "마케팅";
        string meetingName = partName + System.DateTime.Now.ToString(" yyyy년 MM월 dd일") + " 회의";

        DocumentReference textRef = db.Collection(meetingName).Document(num.ToString());
        textRef.SetAsync(textData).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Updated Counter");
            // GetData();
        });
    }
}
