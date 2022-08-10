using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using APIDTO;
using System.Runtime.InteropServices;


public class APIManager : MonoBehaviour
{
    public NetworkSetting setting;

    private UIManager uiManager;

    [DllImport("__Internal")]
    private static extern void ToNextPage(int museId);

    [DllImport("__Internal")]
    private static extern void ConsoleLog(string msg);

    void Awake()
    {
        uiManager = GetComponent<UIManager>();
    }

    public SignInResponse museCredentials;
    public RegisterResponse verseInfo;

    public void RefreshPage()
    {
        ToNextPage(uiManager.roomId);
    }

    private void HandleSignInResp(string resp)
    {
        SignInResponseWrapper response = JsonUtility.FromJson<SignInResponseWrapper>(resp);
        PlayerPrefs.SetString("cred", JsonUtility.ToJson(response.message));
        museCredentials = response.message;
        uiManager.OnSignInResponse(response.message);
        StartCoroutine(Register(response.message.id, response.message.userAlias));
    }

    private void HandleRegisterResp(string resp)
    {
        RegisterResponse response = JsonUtility.FromJson<RegisterResponse>(resp);
        verseInfo = response;
    }

    private void HandleMusicianResp(string resp)
    {
        GetMusicianResponse response = JsonUtility.FromJson<GetMusicianResponse>(resp);
        uiManager.OnMusicianInfo(response);
    }

    private void HandleRandomResp(string resp)
    {
        RandomResponse response = JsonUtility.FromJson<RandomResponse>(resp);
        ToNextPage(response.museId);
    }

    public IEnumerator SendCode(string userNumber)
    {
        MuseSignInRequest req = new MuseSignInRequest();
        req.userNumber = userNumber;

        string json = JsonUtility.ToJson(req);

        using (UnityWebRequest request = UnityWebRequest.Post(setting.MUSEURL + "/api/auth/phone/code", json))
        {
            byte[] rawJson = new UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(rawJson);

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                ConsoleLog(request.downloadHandler.text);
            }
        }
    }

    public IEnumerator SignIn(string userNumber, string code)
    {
        SignInRequest req = new SignInRequest();
        req.userNumber = userNumber;
        req.verificationCode = code;

        PlayerPrefs.SetString("phone", userNumber);

        string json = JsonUtility.ToJson(req);

        using (UnityWebRequest request = UnityWebRequest.Post(setting.MUSEURL + "/api/auth/phone/signin", json))
        {
            byte[] rawJson = new UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(rawJson);

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                HandleSignInResp(request.downloadHandler.text);
            }
        }
    }

    public IEnumerator AutoSignIn()
    {
        string cred = PlayerPrefs.GetString("cred");
        SignInResponse response = JsonUtility.FromJson<SignInResponse>(cred);
        AutoSignInRequest req = new AutoSignInRequest();
        req.userNumber = PlayerPrefs.GetString("phone");
        req.refreshToken = response.refreshToken;
        string json = JsonUtility.ToJson(req);

        using (UnityWebRequest request = UnityWebRequest.Post(setting.MUSEURL + "/api/auth/phone/auto-signin", json))
        {
            byte[] rawJson = new UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(rawJson);

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                HandleSignInResp(request.downloadHandler.text);
            }
        }
    }

    public IEnumerator Register(int museId, string museAlias)
    {
        RegisterRequest req = new RegisterRequest();
        req.museId = museId;
        req.museAlias = museAlias;

        string json = JsonUtility.ToJson(req);

        using (UnityWebRequest request = UnityWebRequest.Post(setting.BASEURL + "/api/user/register", json))
        {
            byte[] rawJson = new UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(rawJson);

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                HandleRegisterResp(request.downloadHandler.text);
            }
        }
    }

    public IEnumerator GetMusician(int id)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(setting.BASEURL + $"/api/user/musician/{id}"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                HandleMusicianResp(request.downloadHandler.text);
            }
        }
    }

    public IEnumerator GetRandomMusician()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(setting.BASEURL + "/api/user/random"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                HandleRandomResp(request.downloadHandler.text);
            }
        }
    }
}
