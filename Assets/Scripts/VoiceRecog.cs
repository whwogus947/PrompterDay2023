using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR;
using static UnityEditor.Progress;

public class VoiceRecog : MonoBehaviour
{
    [SerializeField] private Dropdown dropdown;
    [Range(1, 60)] public int maxRecordingTime = 10;
    public Button recordStart;
    public Button recordEnd;
    public Button convert;
    public AudioSource audioPlayer;
    public AudioClip existClip;

    private AudioClip clip;
    private readonly string fileName = "output.wav";
    private bool isRecording = false;

    private void Start()
    {
        isRecording = false;


        foreach (var device in Microphone.devices)
        {
            dropdown.options.Add(new Dropdown.OptionData(device));
        }
        dropdown.onValueChanged.AddListener(ChangeMicrophone);

        var index = PlayerPrefs.GetInt("user-mic-device-index");
        dropdown.SetValueWithoutNotify(index);

    }

    private void ChangeMicrophone(int index)
    {
        PlayerPrefs.SetInt("user-mic-device-index", index);
    }

    public void StartRecording()
    {
        recordStart.interactable = false;
        recordEnd.interactable = true;
        if (isRecording)
        {
            return;
        }
        var index = PlayerPrefs.GetInt("user-mic-device-index");
        Debug.Log(dropdown.options[index].text);
        clip = Microphone.Start(dropdown.options[index].text, false, maxRecordingTime, 44100);

        isRecording = true;
        StartCoroutine(Recording());
    }

    public void EndRecording()
    {
        recordStart.interactable = true;
        recordEnd.interactable = false;
        var index = PlayerPrefs.GetInt("user-mic-device-index");
        Microphone.End(dropdown.options[index].text);
        audioPlayer.clip = clip;
        audioPlayer.Play();
        isRecording = false;
    }

    public void ConvertWithExistClip()
    {
        Debug.Log(existClip.frequency);
        audioPlayer.clip = existClip;
        audioPlayer.Play();
        byte[] data = SaveWav.Save(fileName, existClip);
        SendRecorded(data).Forget();
    }

    private async UniTaskVoid CreateAudio()
    {
        byte[] data = SaveWav.Save(fileName, clip);
        await SendRecorded(data);
        //RecognizeSpeech(data).Forget();
    }

    IEnumerator Recording()
    {
        yield return new WaitUntil(() => !isRecording);
        CreateAudio().Forget();
    }

    private static readonly string apiKey = "AIzaSyDavMwOAJA5-kwmSYgCT-jq7i5yimjPOjQ";
    private string sttUrl = "https://speech.googleapis.com/v1/speech:recognize";
    //private readonly string endpoint = "https://texttospeech.googleapis.com/v1beta1/speech:recognize?key=";

    private const string apiUrl = "https://speech.googleapis.com/v1p1beta1/speech:recognize?key=AIzaSyDavMwOAJA5-kwmSYgCT-jq7i5yimjPOjQ";

    private async UniTask SendRecorded(byte[] audioData)
    {
        // Prepare the request data
        RecognitionRequest request = new RecognitionRequest
        {
            config = new RecognitionConfig
            {
                encoding = "LINEAR16",
                sampleRateHertz = clip.frequency,
                languageCode = "en-US",
                enableWordConfidence = true,
                enableWordTimeOffsets = true,
                model = "video",
                maxAlternatives = 1,
                enableAutomaticPunctuation = true,
                diarizationConfig = new DiarizationConfig(true, 2, 2),
            },
            audio = new RecognitionAudio
            {
                content = System.Convert.ToBase64String(audioData)
            }
        };

        string requestData = JsonUtility.ToJson(request);
        Debug.Log("Request Data\n" + requestData);
        byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(requestData);

        // Send the POST request to the Google Cloud Speech-to-Text API
        using (UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(requestBytes);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            await webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Response: \n" + jsonResponse);
                ExtractSpeakerTagsAndTranscript(jsonResponse);
                //var result = ExtractTranscriptFromResponse(jsonResponse);
                //for (int i = 0; i < result.Length; i++)
                //{
                //    Debug.Log($"{result[i]}\n");
                //}
            }
        }
    }

    [System.Serializable]
    private class RecognitionRequest
    {
        public RecognitionConfig config;
        public RecognitionAudio audio;
    }

    [System.Serializable]
    private class RecognitionConfig
    {
        public string encoding;
        public int sampleRateHertz;
        public string languageCode;
        public string model;
        public bool enableWordTimeOffsets;
        public bool enableWordConfidence;
        public bool enableAutomaticPunctuation;
        public int maxAlternatives;
        public DiarizationConfig diarizationConfig;
    }

    [System.Serializable]
    private class DiarizationConfig
    {
        public bool enableSpeakerDiarization;
        public int minSpeakerCount;
        public int maxSpeakerCount;

        public DiarizationConfig(bool enableSpeakerDiarization, int minSpeakerCount, int maxSpeakerCount)
        {
            this.enableSpeakerDiarization = enableSpeakerDiarization;
            this.minSpeakerCount = minSpeakerCount;
            this.maxSpeakerCount = maxSpeakerCount;
        }
    }

    [System.Serializable]
    private class RecognitionAudio
    {
        public string content;
    }

    private string[] ExtractTranscriptFromResponse(string jsonResponse)
    {
        RecognitionResponse response = JsonUtility.FromJson<RecognitionResponse>(jsonResponse);

        if (response.results.Length > 0)
        {
            var val = new string[response.results.Length];
            for (int i = 0; i < val.Length; i++)
            {
                val[i] = response.results[i].alternatives[0].transcript;
            }
            // Assuming that the first result contains the transcript
            return val;
        }
        else
        {
            return null;
        }
    }

    [System.Serializable]
    private class RecognitionResponse
    {
        public RecognitionResult[] results;
    }

    [System.Serializable]
    private class RecognitionResult
    {
        public RecognitionAlternative[] alternatives;
    }

    [System.Serializable]
    private class RecognitionAlternative
    {
        public string transcript;
        public RecognitionWord[] words = new RecognitionWord[0];
        // ... (other fields you might want to extract)
    }

    [System.Serializable]
    private class RecognitionWord
    {
        public string word;
        public int speakerTag;
        // ... (other fields you might want to extract)
    }

    private void ExtractSpeakerTagsAndTranscript(string jsonResponse)
    {
        RecognitionResponse response = JsonUtility.FromJson<RecognitionResponse>(jsonResponse);

        Dictionary<int, string> dialogue = new Dictionary<int, string>();

        foreach (var result in response.results)
        {
            foreach (var alternative in result.alternatives)
            {
                string transcript = alternative.transcript;
                Debug.Log("Transcript: " + transcript);

                if (alternative.words != null)
                {
                    foreach (var wordInfo in alternative.words)
                    {
                        int speakerTag = wordInfo.speakerTag;
                        string word = wordInfo.word;

                        if (!dialogue.ContainsKey(speakerTag))
                        {
                            dialogue.Add(speakerTag, word + " ");
                        }
                        else
                        {
                            dialogue[speakerTag] += word + " ";
                        }
                    }
                    foreach (var item in dialogue)
                    {
                        Debug.Log($"{item.Key} : {item.Value}\n");
                    }
                    Debug.Log("CLEAR");
                    dialogue.Clear();
                }
            }
        }
        
    }

    //***********************************************************************************

    public static string ConvertAudioToBase64(byte[] audioBytes)
    {
        try
        {
            string base64Audio = Convert.ToBase64String(audioBytes);
            return base64Audio;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error converting audio to Base64: " + e.Message);
            return null;
        }
    }

    private async UniTaskVoid RecognizeSpeech(byte[] audioBytes)
    {
        // Set up the request headers
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");

        // Set up the request payload
        string audioBase64 = ConvertAudioToBase64(audioBytes); // Implement this function to convert your audio to base64
        string requestJson = @"{
            'config': {
                'encoding': 'LINEAR16',
                'sampleRateHertz': 16000,
                'languageCode': 'en-US'
            },
            'audio': {
                'content': '" + audioBase64 + @"'
            }
        }";

        byte[] requestData = Encoding.UTF8.GetBytes(requestJson);

        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint + apiKey);
        //request.Method = "POST";
        //request.ContentType = "application/json";
        //request.ContentLength = requestData.Length;

        //try
        //{
        //    using (var stream = request.GetRequestStream())
        //    {
        //        stream.Write(requestData, 0, requestData.Length);
        //        stream.Flush();
        //        stream.Close();
        //    }

        //    var resonse = (HttpWebResponse)request.GetResponse();
        //    print(resonse);
        //    //StreamReader reader = new StreamReader(resonse.GetResponseStream());
        //    //string json = reader.ReadToEnd();

        //    //Debug.Log(json);
        //}
        //catch (WebException e)
        //{
        //    Debug.LogException(e);
        //}


        UnityWebRequest request = new UnityWebRequest(sttUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(requestData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "?key=" + apiKey);
        request.SetRequestHeader("ContentType", "application/json");

        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("STT Request failed: " + request.error);
        }
        else
        {
            string responseJson = request.downloadHandler.text;
            // Process the responseJson to extract the recognized text
            Debug.Log("STT Response: " + responseJson);
        }
    }

    //private async UniTaskVoid RecognizeSpeech(byte[] audioBytes)
    //{
    //    // Set up the request headers
    //    Dictionary<string, string> headers = new Dictionary<string, string>();
    //    headers.Add("Content-Type", "application/json");

    //    // Set up the request payload
    //    string audioBase64 = ConvertAudioToBase64(audioBytes); // Implement this function to convert your audio to base64
    //    string requestJson = @"{
    //        'config': {
    //            'encoding': 'LINEAR16',
    //            'sampleRateHertz': 16000,
    //            'languageCode': 'en-US'
    //        },
    //        'audio': {
    //            'content': '" + audioBase64 + @"'
    //        }
    //    }";

    //    byte[] requestData = Encoding.UTF8.GetBytes(requestJson);

    //    // Create and send the request
    //    UnityWebRequest request = new UnityWebRequest(endpoint, "POST");
    //    request.uploadHandler = new UploadHandlerRaw(requestData);
    //    request.downloadHandler = new DownloadHandlerBuffer();
    //    request.SetRequestHeader("Authorization", "Bearer " + apiKey);
    //    request.SetRequestHeader("Content-Type", "application/json");

    //    await request.SendWebRequest();

    //    if (request.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.LogError("STT Request failed: " + request.error);
    //    }
    //    else
    //    {
    //        string responseJson = request.downloadHandler.text;
    //        // Process the responseJson to extract the recognized text
    //        Debug.Log("STT Response: " + responseJson);
    //    }
    //}
}
