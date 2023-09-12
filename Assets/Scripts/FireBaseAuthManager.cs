using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;

public class FireBaseAuthManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;
    
    public InputField email;
    public InputField password;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    // Update is called once per frame
public void SignUp()
{
    auth.CreateUserWithEmailAndPasswordAsync(email.text, password.text).ContinueWith(task => {
        if(task.IsCanceled)
        {
            Debug.LogError("회원가입 취소");
            return;
        }
        if(task.IsFaulted)
        {
            Debug.LogError("회원가입 실패");
            return;
        }

        AuthResult result = task.Result;
        Debug.LogFormat("Firebase user created successfully: {0} ({1})",
        result.User.DisplayName, result.User.UserId);
    });
}
public void Login()
{
    auth.SignInWithEmailAndPasswordAsync(email.text, password.text).ContinueWith(task => {
        if(task.IsCanceled)
        {
            Debug.LogError("로그인 취소");
            return;
        }
        if(task.IsFaulted)
        {
            Debug.LogError("로그인 실패");
            return;
        }
        AuthResult result = task.Result;
        Debug.LogFormat("User signed in successfully: {0} ({1})",
        result.User.DisplayName, result.User.UserId);
    });
}

public void LogOut()
{
    auth.SignOut();
    Debug.Log("로그아웃");
}

}
