using Cysharp.Threading.Tasks;
using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class AvatarGenerator : MonoBehaviour
{
    public ServerStorageManager storageManager;
    [SerializeField] private InputField inputField;
    [SerializeField] private Button button;
    [SerializeField] private Image image;

    private OpenAIApi openai = new OpenAIApi("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");

    private void Start()
    {
        button.onClick.AddListener(() => SetImage());

    }

    private void SetImage()
    {
        Debug.Log("Generating...");
        SendImageRequest().Forget();
    }

    private async UniTask SendImageRequest()
    {
        image.sprite = null;
        button.interactable = false;
        inputField.enabled = false;

        var response = await openai.CreateImage(new CreateImageRequest
        {
            Prompt = inputField.text,
            Size = ImageSize.Size256
        });

        if (response.Data != null && response.Data.Count > 0)
        {
            using (var request = new UnityWebRequest(response.Data[0].Url))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Access-Control-Allow-Origin", "*");
                await request.SendWebRequest();

                while (!request.isDone) await UniTask.Yield();

                Texture2D texture = new Texture2D(32, 32);
                texture.LoadImage(request.downloadHandler.data);
                var sprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), Vector2.zero, 1f);
                image.sprite = sprite;

                byte[] bytes = texture.EncodeToJPG();
                storageManager.UploadImage(bytes);
            }
        }
        else
        {
            Debug.LogWarning("No image was created from this prompt.");
        }

        button.interactable = true;
        inputField.enabled = true;
    }
}
