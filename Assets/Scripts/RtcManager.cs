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

    [DllImport("__Internal")]
    private static extern void SetVolume(string uid, int volume);

    public int globalVolume = 100;
    public float threshold = 5f;


    private APIManager apiManager;
    private PlayerManager playerManager;

    private string channelName;
    private string account;

    public void SetGlobalVolume(float val)
    {
        globalVolume = Mathf.FloorToInt(100f * val);
    }

    void Awake()
    {
        apiManager = GetComponent<APIManager>();
        playerManager = GetComponent<PlayerManager>();
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

    void Update()
    {
        foreach (string sid in playerManager.others.Keys)
        {
            Transform tf = playerManager.others[sid];
            float distance = (playerManager.me.transform.position - tf.position).magnitude;
            float ratio = Mathf.Min(1f, 1f / Mathf.Pow(distance, 2));
            int volume = Mathf.FloorToInt(ratio * globalVolume);
            if (distance > threshold)
            {
                volume = 0;
            }
            SetVolume(sid, volume);
        }
    }
}
