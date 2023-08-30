using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JsonConverter : MonoBehaviour
{
    void Start()
    {
        
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
        string jsonFilePath = "city"; // Without the file extension
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(jsonFilePath);
        string jsonText = jsonTextAsset.text;
        Debug.Log(Convert(jsonText));
    }


    public static GPTResult Convert(string json)
    {
        if (string.IsNullOrEmpty(json))
            return null;

        GPTResult result = JsonUtility.FromJson<GPTResult>(json);
        
        //Debug.Log(result.title);
        //Debug.Log(result.item.Length);
        //Debug.Log(result.conclusion);

        return result;
    }

    [System.Serializable]
    public class GPTResult
    {
        public string title;
        public Item[] item;
        public string conclusion;
    }

    [System.Serializable]
    public class Item
    {
        public string speaker;
        public string message;
        public string time;

        public Item(string speaker, string message, string time)
        {
            this.speaker = speaker;
            this.message = message;
            this.time = time;
        }
    }
}
