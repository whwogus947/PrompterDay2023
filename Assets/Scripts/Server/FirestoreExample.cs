using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Extensions;
using Cysharp.Threading.Tasks;

public class FirestoreExample : MonoBehaviour
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
        string text = "�ȳ��ϼ���";
        System.DateTime localDate = System.DateTime.Now;
        string formattedTime = localDate.ToString("yyyy.MM.dd HH:mm:ss");

        sendTextToServer(name, text, formattedTime);

    }

    public void LogAllData()
    {
        GetData().Forget();
    }

    private async UniTaskVoid GetData()
    {
        //db.Collection("User").Document("Counters").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        //{
        //    Counters counter = task.Result.ConvertTo<Counters>();
        //    countUI.text = counter.Count.ToString();
        //});

        Query allCitiesQuery = db.Collection("������ 2023�� 08�� 27�� ȸ��");
        QuerySnapshot allCitiesQuerySnapshot = await allCitiesQuery.GetSnapshotAsync();

        foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
        {
            Dictionary<string, object> city = documentSnapshot.ToDictionary();
            foreach (KeyValuePair<string, object> pair in city)
            {
                Debug.Log(pair.Key + " " + pair.Value);
            }
        }
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
        string partName = "������";
        string meetingName = partName + System.DateTime.Now.ToString(" yyyy�� MM�� dd��") + " ȸ��";

        DocumentReference textRef = db.Collection(meetingName).Document(num.ToString());
        textRef.SetAsync(textData).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Updated Counter");
            // GetData();
        });
    }
}
