using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;

public class RtcManager : MonoBehaviour
{
    [SerializeField]
    private string appId = "app_id";

    private IRtcEngine mRtcEngine = null;

    private APIManager apiManager;

    private string channelName;
    private string account;

    void Awake()
    {
        apiManager = GetComponent<APIManager>();
    }

    void OnApplicationQuit()
    {
        if (mRtcEngine != null)
        {
            // Destroy the IRtcEngine object.
            IRtcEngine.Destroy();
            mRtcEngine = null;
        }
    }

    // Start is called before the first frame update
    public void Initialize()
    {
        mRtcEngine = IRtcEngine.GetEngine(appId);

        mRtcEngine.OnJoinChannelSuccess += (string channelName, uint uid, int elapsed) =>
        {
            string joinSuccessMessage = string.Format("joinChannel callback uid: {0}, channel: {1}", uid, channelName);
            Debug.Log(joinSuccessMessage);
        };

        mRtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_COMMUNICATION);
    }

    public void Join(string account, string channelName)
    {
        this.account = account;
        this.channelName = channelName;
        StartCoroutine(apiManager.GetRtcToken(channelName, account));
    }

    public void OnTokenResponse(string token)
    {
        mRtcEngine.JoinChannelWithUserAccount(token, channelName, account);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
