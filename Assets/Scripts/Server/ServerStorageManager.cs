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

    void LoadImage()
    {
        rawImage = gameObject.GetComponent<RawImage>();

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

    private void Start()
    {
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://prompterdayseoul2023.appspot.com/");
    }

    public void UploadImage(byte[] bytes)
    {
        var newMetadata = new MetadataChange();
        newMetadata.ContentType = "image/jpg";

        StorageReference uploadRef = storageReference.Child("Avatars/user" + UnityEngine.Random.Range(0, 9999) + System.DateTime.Now.ToString("ss") + ".jpg");
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
