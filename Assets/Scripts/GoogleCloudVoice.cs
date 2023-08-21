using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace NousPlayerCharacter
{
    // ****************************************************************************************************************
    // ******************************************** TTS for Nous Player Character *************************************
    // ****************************************************************************************************************

    [RequireComponent(typeof(AudioSource))]
    public class GoogleCloudVoice : MonoBehaviour
    {
        public AudioSource audioPlayer;
        private Setup setup;
        private readonly string endpoint = "https://texttospeech.googleapis.com/v1beta1/text:recognize?key=";
        private readonly string key = "AIzaSyDavMwOAJA5-kwmSYgCT-jq7i5yimjPOjQ";

        private void Start()
        {

        }

        public void Play(string msg)
        {
            //PlayVoice(msg).Forget();
            UseDefaultSetting(msg);
            PlayAudio(CreateAudioClip());
        }

        //private async UniTask PlayVoice(string msg)
        //{
        //    UseDefaultSetting(msg);
        //    PlayAudio(CreateAudioClip());
        //}

        private void PlayAudio(AudioClip clip)
        {
            audioPlayer.clip = clip;
            audioPlayer.Play();
        }

        private void UseDefaultSetting(string msg)
        {
            if (setup != null)
            {
                setup.input.text = msg;
                return;
            }
            setup = new
                (
                message: msg,
                language: "ko-KR",
                model: "ko-KR-Standard-B",
                gender: "FEMALE",
                encoding: "LINEAR16",
                rate: 1.2f,
                pitch: 0,
                gain: 0
                );
            setup.voice.diarizationConfig = new DiarizationConfig();
            setup.voice.diarizationConfig.enableSpeakerDiarization = true;
            setup.voice.diarizationConfig.maxSpeakerCount = 2;
            setup.voice.diarizationConfig.minSpeakerCount = 1;
        }

        private AudioClip CreateAudioClip()
        {
            var str = TextToSpeechPost(setup);
            GetContent info = JsonUtility.FromJson<GetContent>(str);

            var bytes = Convert.FromBase64String(info.audioContent);
            var f = ConvertByteToFloat(bytes);

            AudioClip audioClip = AudioClip.Create("audioContent", f.Length, 1, 44100, false);
            audioClip.SetData(f, 0);

            return audioClip;
        }

        private float[] ConvertByteToFloat(byte[] array)
        {
            float[] floatArr = new float[array.Length / 2];

            for (int i = 0; i < floatArr.Length; i++)
            {
                floatArr[i] = BitConverter.ToInt16(array, i * 2) / 32768.0f;
            }

            return floatArr;
        }

        private string TextToSpeechPost(object data)
        {
            string str = JsonUtility.ToJson(data);
            var bytes = System.Text.Encoding.UTF8.GetBytes(str);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint + key);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = bytes.Length;

            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                    stream.Close();
                }

                var resonse = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(resonse.GetResponseStream());
                string json = reader.ReadToEnd();

                return json;
            }
            catch (WebException e)
            {
                Debug.LogException(e);
                return null;
            }
        }
    }

    [System.Serializable]
    public class Setup
    {
        public Message input;
        public VoiceConfig voice;
        public AudioConfig audioConfig;

        public Setup(string message, string language, string model, string gender, string encoding, float rate, int pitch, int gain)
        {
            input = new(message);
            voice = new(language, model, gender);
            audioConfig = new(encoding, rate, pitch, gain);
        }
    }

    [System.Serializable]
    public class Message
    {
        public string text;
        public Message(string text)
        {
            this.text = text;
        }
    }

    [System.Serializable]
    public class VoiceConfig
    {
        public string languageCode;
        public DiarizationConfig diarizationConfig;
        public string name;
        public string ssmlGender;

        public VoiceConfig(string languageCode, string name, string ssmlGender)
        {
            this.languageCode = languageCode;
            this.name = name;
            this.ssmlGender = ssmlGender;
        }
    }

    [System.Serializable]
    public class DiarizationConfig
    {
        public bool enableSpeakerDiarization;
        public int minSpeakerCount;
        public int maxSpeakerCount;
    }

    [System.Serializable]
    public class AudioConfig
    {
        public string audioEncoding;
        public float speakingRate;
        public int pitch;
        public int volumeGainDb;

        public AudioConfig(string audioEncoding, float speakingRate, int pitch, int volumeGainDb)
        {
            this.audioEncoding = audioEncoding;
            this.speakingRate = speakingRate;
            this.pitch = pitch;
            this.volumeGainDb = volumeGainDb;
        }
    }

    [System.Serializable]
    public class GetContent
    {
        public string audioContent;
    }
}