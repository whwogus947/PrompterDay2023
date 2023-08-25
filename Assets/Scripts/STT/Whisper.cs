using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OpenAI
{
    public class Whisper : MonoBehaviour
    {
        [SerializeField] private Dropdown dropdown;
        public int duration = 120;
        
        private readonly string fileName = "output.wav";
        
        private AudioClip clip;
        private bool isRecording;
        private float time;
        private OpenAIApi openai = new OpenAIApi("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");

        private void Start()
        {
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
        
        public void RecordingStart()
        {
            time = 0f;
            isRecording = true;

            var index = PlayerPrefs.GetInt("user-mic-device-index");
            clip = Microphone.Start(dropdown.options[index].text, false, duration, 44100);
        }

        public void RecordingEnd()
        {
            time = 0;
            isRecording = false;
            EndRecording().Forget();
        }

        private async UniTaskVoid EndRecording()
        {
            Debug.Log("Transcripting...");

            Microphone.End(dropdown.options[0].text);
            byte[] data = SaveWav.Save(fileName, clip);
            
            var req = new CreateAudioTranscriptionsRequest
            {
                FileData = new FileData() {Data = data, Name = "audio.wav"},
                // File = Application.persistentDataPath + "/" + fileName,
                Model = "whisper-1",
                Language = "ko"
            };
            var res = await openai.CreateAudioTranscription(req);

            Debug.Log("RESULT : " + res.Text);
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
