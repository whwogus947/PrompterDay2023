using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JsonConverter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Convert(string json)
    {
        if (string.IsNullOrEmpty(json))
            return;
        GPTResult result = JsonUtility.FromJson<GPTResult>(json);
        
        Debug.Log(result.title);
        Debug.Log(result.item.Length);
        Debug.Log(result.conclusion);
    }

    [System.Serializable]
    public class GPTResult
    {
        public Title title;
        public Item[] item;
        public Conclusion conclusion;
    }

    [System.Serializable]
    public class Title
    {
        public string title;
        public Title(string text)
        {
            this.title = text;
        }
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

    [System.Serializable]
    public class Conclusion
    {
        public string conclusion;

        public Conclusion(string conclusion)
        {
            this.conclusion = conclusion;
        }
    }
}
