using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;

public class ServerStorageManager : MonoBehaviour
{
    RawImage rawImage;
    FirebaseStorage storage;
    StorageReference storageReference;

    IEnumerator LoadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Create a request
        yield return request.SendWebRequest(); //Wait for the request to complete
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            // setting the loaded image to our object
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rawImage = gameObject.GetComponent<RawImage>();
        // StartCoroutine(LoadImage("https://firebasestorage.googleapis.com/v0/b/vikings-login.appspot.com/o/VikingsLogo.jpeg?alt=media&token=44b39032-34fe-489e-bd69-536f0ed7c251"));
        //Easy hardcoded solution But bad approach

        //initialize storage reference
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://prompterdayseoul2023.appspot.com/");

        //get reference of image
        StorageReference image = storageReference.Child("test.png");

        //Get the download link of file
        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                StartCoroutine(LoadImage(Convert.ToString(task.Result))); //Fetch file from the link
            }
            else
            {
                Debug.Log(task.Exception);
            }
        });
    }

    public void OnButtonClick()
    {
        ShowLoadDialogCoroutine();
    }

    private void ShowLoadDialogCoroutine()
    {
        //byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);
        byte[] bytes = null;

        var newMetadata = new MetadataChange();
        newMetadata.ContentType = "image/jpeg";

        StorageReference uploadRef = storageReference.Child("uploads/newFile.jpeg");
        Debug.Log("File upload started");
        uploadRef.PutBytesAsync(bytes, newMetadata).ContinueWithOnMainThread((task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
            }
            else
            {
                Debug.Log("File Uploaded Successfully!");
            }
        });
    }
}
