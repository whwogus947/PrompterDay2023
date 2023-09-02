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
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class FirestoreExample : MonoBehaviour
{
    public static FirestoreExample Inst { get; private set; }
    public static bool IsConferenceEnd = false;

    public GameObject resultBox;
    public Whisper whisper;
    public ContentVisualManager visualizer;

    private FirebaseFirestore db;
    private ListenerRegistration listenerRegistration;

    private void Awake()
    {
        IsConferenceEnd = false;

        if (Inst == null)
            Inst = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        //updateCountButton.onClick.AddListener(OnHandleClick);
        


        // GetData();
        //LogAllData();
        //SendRoomDataToServer("Room1232", false, "test");
        //GetRoomInformation().Forget();
    }

    void OnDestroy()
    {
        listenerRegistration.Stop();
    }

    public void OpenListener()
    {
        SendRoomDataToServer(ChatRoomMaker.RoomName, false, "");

        listenerRegistration = db.Collection(ChatRoomMaker.RoomName).Document("ConferenceInfo").Listen(snapshot =>
        {
            ConferenceInfo info = snapshot.ConvertTo<ConferenceInfo>();
            IsConferenceEnd = info.IsConferenceEnd;
            resultBox.SetActive(IsConferenceEnd);
            if (IsConferenceEnd)
            {
                whisper.gameObject.SetActive(false);
                visualizer.visualizerTarget = info.FormedResult;
            }
            else
            {
                whisper.gameObject.SetActive(true);
            }
        });
    }

    public void EndConference(string summary)
    {
        SendRoomDataToServer(ChatRoomMaker.RoomName, true, summary);
    }

    public async UniTaskVoid GetRoomInformation()
    {
        ConferenceInfo roomInfo = await GetConferenceInformation("Room1232");
        Debug.Log(roomInfo.IsConferenceEnd);
        Debug.Log(roomInfo.FormedResult);
    }

    public void Send(string channelName, string userName, string message)
    {
        string name = userName;
        string text = message;
        System.DateTime localDate = System.DateTime.Now;
        string formattedTime = localDate.ToString("yyyy-MM-dd HH-mm-ss");

        SendTextToServer(channelName, name, text, formattedTime);
    }

    public void LogAllData(string roomName = "Room1232")
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
            if (documentSnapshot.Id == "ConferenceInfo")
            {
                continue;
            }
            Dictionary<string, object> city = documentSnapshot.ToDictionary();
            foreach (KeyValuePair<string, object> pair in city)
            {
                DataOrganizer.FieldsData[pair.Key] = pair.Value.ToString();
            }
            string fieldData = $"[{DataOrganizer.FieldsData["Time"]}] [{DataOrganizer.FieldsData["UserName"]}] [{DataOrganizer.FieldsData["Text"]}]";
            result += fieldData + "\n";
        }
        Debug.Log(result);

        string msg = await ChatDeliever.Inst.Send(result);
        EndConference(msg);
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
            Debug.Log("Text Updated");
        });
    }

    private void SendRoomDataToServer(string channelName, bool isEnd, string text)
    {
        ConferenceInfo textData = new ConferenceInfo
        {
            IsConferenceEnd = isEnd,
            FormedResult = text,
        };

        DocumentReference textRef = db.Collection(channelName).Document("ConferenceInfo");
        textRef.SetAsync(textData).ContinueWithOnMainThread(task =>
        {
            Debug.Log("ConferenceInfo Updated");
        });
    }

    private async UniTask<ConferenceInfo> GetConferenceInformation(string channelName)
    {
        DocumentSnapshot snapshot = await db.Collection(channelName).Document("ConferenceInfo").GetSnapshotAsync();
        ConferenceInfo info = new ConferenceInfo();

        if (snapshot.Exists)
        {
            Dictionary<string, object> data = snapshot.ToDictionary();

            if (data.ContainsKey("IsConferenceEnd"))
                info.IsConferenceEnd = Boolean.Parse(data["IsConferenceEnd"].ToString());
            else if (data.ContainsKey("FormedResult"))
                info.FormedResult = data["FormedResult"].ToString();

            info = new ConferenceInfo(Boolean.Parse(data["IsConferenceEnd"].ToString()), data["FormedResult"].ToString());
            return info;
        }
        else
        {
            Debug.Log("No Conference Data");
        }
        return info;
    }
}

[FirestoreData]
public struct ConferenceInfo
{
    [FirestoreProperty]
    public bool IsConferenceEnd { get; set; }
    [FirestoreProperty]
    public string FormedResult { get; set; }

    public ConferenceInfo(bool isConferenceEnd, string formedResult)
    {
        IsConferenceEnd = isConferenceEnd;
        FormedResult = formedResult;
    }
}