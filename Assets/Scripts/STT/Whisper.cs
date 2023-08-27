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
        public AudioSource audio;

        private readonly string fileName = "output.wav";
        
        private AudioClip clip;
        private bool isRecording;
        private float time;
        private OpenAIApi openai = new OpenAIApi("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");
        private float debugTimer = 0f;

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

        private void RecordAsSample(string deviceName, AudioClip recordedClip)
        {
            var position = Microphone.GetPosition(deviceName);
            Debug.Log("position : " + position);
            var soundData = new float[recordedClip.samples * recordedClip.channels];
            recordedClip.GetData(soundData, 0);

            //Create shortened array for the data that was used for recording
            var newData = new float[position * recordedClip.channels];

            //Copy the used samples to a new array
            for (int i = 0; i < newData.Length; i++)
            {
                newData[i] = soundData[i];
            }

            var newClip = AudioClip.Create(recordedClip.name, position, recordedClip.channels, recordedClip.frequency, false);
            newClip.SetData(newData, 0);
            Destroy(recordedClip);

            Debug.Log(newClip.length);
            clip = newClip;
        }

        public void RecordingEnd()
        {
            Debug.Log("Timer : " + debugTimer);
            debugTimer = 0f;
            time = 0;
            isRecording = false;
            EndRecording().Forget();
        }

        private async UniTaskVoid EndRecording()
        {
            Debug.Log("Transcripting...");
            var index = PlayerPrefs.GetInt("user-mic-device-index");
            RecordAsSample(dropdown.options[index].text, clip);
            Microphone.End(dropdown.options[index].text);

            audio.clip = clip;
            audio.Play();

            //byte[] data = SaveWav.Save(fileName, clip);

            //var req = new CreateAudioTranscriptionsRequest
            //{
            //    FileData = new FileData() {Data = data, Name = "audio.wav"},
            //    // File = Application.persistentDataPath + "/" + fileName,
            //    Model = "whisper-1",
            //    Language = "ko"
            //};
            //var res = await openai.CreateAudioTranscription(req);

            //Debug.Log("RESULT : " + res.Text);
        }

        private void Update()
        {
            if (isRecording)
            {
                time += Time.deltaTime;
                debugTimer = time;
                if (time >= duration)
                {
                    RecordingEnd();
                }
            }
        }
    }
}
