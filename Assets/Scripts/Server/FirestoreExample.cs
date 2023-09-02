using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Extensions;
using Cysharp.Threading.Tasks;
using System;
using OpenAI;

public class FirestoreExample : MonoBehaviour
{
    public static FirestoreExample Inst { get; private set; }

    private FirebaseFirestore db;
    private ListenerRegistration listenerRegistration;

    private void Awake()
    {
        if (Inst == null)
            Inst = this;
        else
            Destroy(gameObject);
    }

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

    public void LogAllData(string roomName = "Room1321")
    {
        GetData(roomName).Forget();
    }

    private async UniTaskVoid GetData(string conferenceName)
    {
        Query allCitiesQuery = db.Collection(conferenceName);
        QuerySnapshot allCitiesQuerySnapshot = await allCitiesQuery.GetSnapshotAsync();

        string result = "";
        foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
        {
            Dictionary<string, object> city = documentSnapshot.ToDictionary();
            foreach (KeyValuePair<string, object> pair in city)
            {
                DataOrganizer.FieldsData[pair.Key] = pair.Value.ToString();
            }
            string fieldData = $"[{DataOrganizer.FieldsData["Time"]}] [{DataOrganizer.FieldsData["UserName"]}] [{DataOrganizer.FieldsData["Text"]}]";
            result += fieldData + "\n";
        }

        ChatDeliever.Inst.Send(result).Forget();
        Debug.Log(result);
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

    private async UniTask<ConferenceInfo> GetConferenceInformation(string channelName)
    {
        DocumentSnapshot snapshot = await db.Collection(channelName).Document("ConferenceInfo").GetSnapshotAsync();

        if (snapshot.Exists)
        {
            Dictionary<string, object> data = snapshot.ToDictionary();

            ConferenceInfo info = new ConferenceInfo();
            if (data.ContainsKey("IsConferenceEnd"))
                info.IsConferenceEnd = Boolean.Parse(data["IsConferenceEnd"].ToString());
            else if (data.ContainsKey("FormedResult"))
                info.FormedResult = data["FormedResult"].ToString();

            return info;
        }
        else
        {
            Debug.Log("No Conference Data");
        }

        ConferenceInfo empty = new ConferenceInfo();
        return empty;
    }
}

[FirestoreData]
public struct ConferenceInfo
{
    [FirestoreProperty]
    public bool IsConferenceEnd { get; set; }
    [FirestoreProperty]
    public string FormedResult { get; set; }
}