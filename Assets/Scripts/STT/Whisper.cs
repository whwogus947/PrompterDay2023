﻿using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OpenAI
{
    public class Whisper : MonoBehaviour
    {
        [SerializeField] private Dropdown dropdown;
        public int duration = 120;
        public FirestoreExample firestore;
        public VivoxVoiceManager voiceManager;
        public bool isMessageToServer = false;
        public string userName;
        public string channelName;

        private readonly string fileName = "output.wav";
        
        private AudioClip clip;
        private bool isRecording;
        private float time;
        private OpenAIApi openai = new OpenAIApi("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");
        private string micDevice = "";

        private void Start()
        {
            micDevice = Microphone.devices[0];

            //foreach (var device in Microphone.devices)
            //{
            //    dropdown.options.Add(new Dropdown.OptionData(device));
            //}
            //dropdown.onValueChanged.AddListener(ChangeMicrophone);

            //var index = PlayerPrefs.GetInt("user-mic-device-index");
            //dropdown.SetValueWithoutNotify(index);
        }

        private void ChangeMicrophone(int index)
        {
            PlayerPrefs.SetInt("user-mic-device-index", index);
        }
        
        public void RecordingStart()
        {
            time = 0f;
            isRecording = true;

            var index = PlayerPrefs.GetInt("user-mic-device-index");
            clip = Microphone.Start(micDevice, false, duration, 44100);
            //clip = Microphone.Start(dropdown.options[index].text, false, duration, 44100);

        }

        private void RecordAsSample(string deviceName, AudioClip recordedClip)
        {
            var position = Microphone.GetPosition(deviceName);

            var soundData = new float[recordedClip.samples * recordedClip.channels];
            recordedClip.GetData(soundData, 0);

            //Create shortened array for the data that was used for recording
            var newData = new float[position * recordedClip.channels];

            //Copy the used samples to a new array
            for (int i = 0; i < newData.Length; i++)
            {
                newData[i] = soundData[i];
            }

            if (position > 0)
            {
                var newClip = AudioClip.Create(recordedClip.name, position, recordedClip.channels, recordedClip.frequency, false);
                newClip.SetData(newData, 0);
                Destroy(recordedClip);

                //Debug.Log(newClip.length);
                clip = newClip;
            }
        }

        public void RecordingEnd()
        {
            time = 0;
            isRecording = false;
            EndRecording().Forget();
        }

        private async UniTaskVoid EndRecording()
        {
            //Debug.Log("Transcripting...");
            var index = PlayerPrefs.GetInt("user-mic-device-index");
            RecordAsSample(micDevice, clip);
            Microphone.End(micDevice);

            byte[] data = SaveWav.Save(fileName, clip);

            var req = new CreateAudioTranscriptionsRequest
            {
                FileData = new FileData() { Data = data, Name = "audio.wav" },
                // File = Application.persistentDataPath + "/" + fileName,
                Prompt = "If the voice is not detected, return empty.",
                Model = "whisper-1",
                Language = "ko"
            };
            var res = await openai.CreateAudioTranscription(req);

            if (!IsSentenceMadeOfSpaces(res.Text) && isMessageToServer)
            {
                firestore.Send(channelName, userName, res.Text);
            }

            Debug.Log("<color=green>RESULT</color> : " + res.Text);
        }

        private bool IsSentenceMadeOfSpaces(string sentence)
        {
            foreach (char c in sentence)
            {
                if (!char.IsWhiteSpace(c))
                {
                    return false;
                }
            }
            return true;
        }

        private void Update()
        {
            if (isRecording)
            {
                time += Time.deltaTime;
                if (time >= duration)
                {
                    RecordingEnd();
                }
            }
        }
    }
}
