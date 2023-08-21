using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class VoiceRecog : MonoBehaviour
{
    [SerializeField] private Dropdown dropdown;
    [Range(1, 20)] public int maxRecordingTime;
    public Button recordStart;
    public Button recordEnd;

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
        isRecording = false;
    }

    private async UniTaskVoid CreateAudio()
    {
        byte[] data = SaveWav.Save(fileName, clip);

    }

    IEnumerator Recording()
    {
        yield return new WaitUntil(() => !isRecording);
        CreateAudio().Forget();
    }

    private string apiKey = "YOUR_GOOGLE_CLOUD_API_KEY";
    private string sttUrl = "https://speech.googleapis.com/v1/speech:recognize";

    public static string ConvertAudioToBase64(string audioFilePath = "")
    {
        try
        {
            byte[] audioBytes = null;
            string base64Audio = Convert.ToBase64String(audioBytes);
            return base64Audio;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error converting audio to Base64: " + e.Message);
            return null;
        }
    }

    private IEnumerator RecognizeSpeech()
    {
        // Set up the request headers
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");

        // Set up the request payload
        string audioBase64 = ConvertAudioToBase64(); // Implement this function to convert your audio to base64
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

        // Create and send the request
        UnityWebRequest request = new UnityWebRequest(sttUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(requestData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

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
}
