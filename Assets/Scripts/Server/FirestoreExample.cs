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
    FirebaseFirestore db;
    ListenerRegistration listenerRegistration;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        //updateCountButton.onClick.AddListener(OnHandleClick);
        //listenerRegistration = db.Collection("User").Document("Counters").Listen(snapshot =>
        //{
        //    Counters counter = snapshot.ConvertTo<Counters>();
        //    countUI.text = counter.Count.ToString();
        //});


        // GetData();
    }

    void OnDestroy()
    {
        //listenerRegistration.Stop();
    }

    public void Send(string channelName, string userName, string message)
    {
        string name = userName;
        string text = message;
        System.DateTime localDate = System.DateTime.Now;
        string formattedTime = localDate.ToString("yyyy-MM-dd HH-mm-ss");

        SendTextToServer(channelName, name, text, formattedTime);
    }

    public void LogAllData()
    {
        GetData().Forget();
    }

    private async UniTaskVoid GetData()
    {
        Query allCitiesQuery = db.Collection("마케팅 2023년 08월 27일 회의");
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

    private void SendTextToServer(string channelName, string userName, string text, string time)
    {
        textDatas textData = new textDatas
        {
            UserName = userName,
            Text = text,
            Time = time
        };

        string detailedTime = System.DateTime.Now.ToString("HH-mm-ss");

        DocumentReference textRef = db.Collection(channelName).Document(detailedTime);
        textRef.SetAsync(textData).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Updated Counter");
        });
    }
}
