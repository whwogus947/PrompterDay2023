using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

public class JsonConverter : MonoBehaviour
{
    void Start()
    {
        // DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public static GPTResult TestAsSample()
    {
        string jsonFilePath = "city"; // Without the file extension
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(jsonFilePath);
        string jsonText = jsonTextAsset.text;
        return Convert(jsonText);
    }

    public void ConvertToJson()
    {
        // string jsonFilePath = "city"; // Without the file extension
        // TextAsset jsonTextAsset = Resources.Load<TextAsset>(jsonFilePath);
        // string jsonText = jsonTextAsset.text;
        // Debug.Log(Convert(jsonText));

        string jsonFilePath = "bakery"; // Without the file extension
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(jsonFilePath);
        string jsonText = jsonTextAsset.text;
        Debug.Log(Convert(jsonText));
        // mDatabaseRef.Child("users").Child(userId).SetRawJsonValueAsync(json);



    }


    public static GPTResult Convert(string json)
    {
        if (string.IsNullOrEmpty(json))
            return null;
        
        GPTResult result = JsonUtility.FromJson<GPTResult>(json);

        // Debug.Log(result.meetingTopic);
        // Debug.Log(result.meetingAgenda);
        // Debug.Log(result.conclusion);

        return result;
    }

    [System.Serializable]
    public class GPTResult
    {
        public string meetingTopic;
        public MeetingAgenda[] meetingAgenda;
        public int selectedIdea;
        public string conclusion;
        public List<string> topicForFurtherDiscussion;
    }

    [System.Serializable]
    public class MeetingAgenda
    {
        public int numberOfIdea;
        public string summaryOfIdea;
        public string speaker;
        public string contentsOfIdea;
        public Comments[] comments;
    
        public MeetingAgenda(int numberOfIdea, string summaryOfIdea, string speaker, string contentsOfIdea, Comments[] comments)
        {
            this.numberOfIdea = numberOfIdea;
            this.summaryOfIdea = summaryOfIdea;
            this.speaker = speaker;
            this.contentsOfIdea = contentsOfIdea;
            this.comments = comments;
        }
    }
    [System.Serializable]
    public class Comments
    {
        public string speaker;
        public string contentsOfComment;
        public bool isPositive;

        public Comments(string speaker, string contentsOfComment, bool isPositive){
            this.speaker = speaker;
            this.contentsOfComment = contentsOfComment;
            this.isPositive = isPositive;
        }
    }

}
