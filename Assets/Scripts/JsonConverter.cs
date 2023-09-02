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
        //string jsonFilePath = "city"; // Without the file extension
        string jsonFilePath = "bakery"; // Without the file extension
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

        Debug.Log(result.MeetingTopic);
        Debug.Log(result.MeetingAgenda);
        Debug.Log(result.Conclusion);

        return result;
    }

    [System.Serializable]
    public class GPTResult
    {
        public string MeetingTopic;
        public MeetingAgenda[] MeetingAgenda;
        public int SelectedIdea;
        public string Conclusion;
        public List<string> TopicForFurtherDiscussion;
    }

    [System.Serializable]
    public class MeetingAgenda
    {
        public int NumberOfIdea;
        public string SummaryOfIdea;
        public string Speaker;
        public string ContentsOfIdea;
        public Comments[] Comments;
    
        public MeetingAgenda(int numberOfIdea, string summaryOfIdea, string speaker, string contentsOfIdea, Comments[] comments)
        {
            this.NumberOfIdea = numberOfIdea;
            this.SummaryOfIdea = summaryOfIdea;
            this.Speaker = speaker;
            this.ContentsOfIdea = contentsOfIdea;
            this.Comments = comments;
        }
    }
    [System.Serializable]
    public class Comments
    {
        public string Speaker;
        public string ContentsOfComment;
        public bool IsPositive;

        public Comments(string speaker, string contentsOfComment, bool isPositive){
            this.Speaker = speaker;
            this.ContentsOfComment = contentsOfComment;
            this.IsPositive = isPositive;
        }
    }

}
