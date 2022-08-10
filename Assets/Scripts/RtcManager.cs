using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketDTO;
using System.Runtime.InteropServices;

public class RtcManager : MonoBehaviour
{
    [SerializeField]
    private string appId = "app_id";

    [DllImport("__Internal")]
    private static extern void JoinRTC(string appId, string channel, string token, string uid);

    private APIManager apiManager;


    private string channelName;
    private string account;

    void Awake()
    {
        apiManager = GetComponent<APIManager>();
    }

    public void Join(string account, string channelName)
    {
        this.account = account;
        this.channelName = channelName;
        StartCoroutine(apiManager.GetRtcToken(channelName, account));
    }

    public void OnTokenResponse(string token)
    {
        JoinRTC(appId, channelName, token, account);
    }
}
