using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.SocketIO3;
using SocketDTO;
public class StudioManager : MonoBehaviour
{
    private SocketManager manager;
    public Socket deviceChannel;
    private APIManager apiManager;
    private UIManager uIManager;
    private NetworkManager networkManager;
    private RtcManager rtcManager;

    void Awake()
    {
        apiManager = GetComponent<APIManager>();
        uIManager = GetComponent<UIManager>();
        networkManager = GetComponent<NetworkManager>();
        rtcManager = GetComponent<RtcManager>();
    }

    void OnConnect()
    {
        Debug.Log("Connected to Studio");
        StudioJoinRequest req = new StudioJoinRequest();
        req.deviceType = "ios";
        req.channelName = apiManager.museCredentials.userAlias;
        deviceChannel.Emit("reqJoinDeviceCh", req);
    }

    // Start is called before the first frame update
    public void Connect()
    {
        SocketOptions options = new SocketOptions();
        options.ConnectWith = BestHTTP.SocketIO3.Transports.TransportTypes.WebSocket;
        options.AutoConnect = false;
        options.AdditionalQueryParams = new PlatformSupport.Collections.ObjectModel.ObservableDictionary<string, string>();
        options.AdditionalQueryParams.Add("auth_token", apiManager.museCredentials.accessToken.Replace("Bearer ", ""));
        manager = new SocketManager(new System.Uri("https://prod.muse.live/channel/socket.io/"), options);
        deviceChannel = manager.GetSocket("/device-channel");
        deviceChannel.On(SocketIOEventTypes.Connect, OnConnect);
        deviceChannel.On(SocketIOEventTypes.Error, OnError);

        deviceChannel.On<StudioJoinResponse>("recJoinDeviceCh", OnMacDiscovered);
        deviceChannel.On<StudioJoinResponse>("recHealthCheck", OnMacDiscovered);
        deviceChannel.On<StudioJoinResponse>("recLeaveDeviceCh", OnMacDisconnected);
        manager.Open();
    }

    void OnMacDisconnected(StudioJoinResponse resp)
    {
        if (resp.deviceType == "macos")
        {
            uIManager.SetPerformButton(false);
        }
    }

    void OnMacDiscovered(StudioJoinResponse resp)
    {
        if (resp.deviceType == "macos")
        {
            StudioJoinResponse req = new StudioJoinResponse();
            req.deviceType = "ios";
            deviceChannel.Emit("reqConnect", req);
            uIManager.SetPerformButton(true);
        }
    }

    public void StartPerform()
    {
        StudioStreamRequest req = new StudioStreamRequest();
        req.streamKey = apiManager.verseInfo.streamKey;
        deviceChannel.Emit("reqStream", req);
        networkManager.root.Emit("perform", true);
        uIManager.reactionBanner.gameObject.SetActive(true);
        uIManager.SetStageMood(true, networkManager.socketId);
        rtcManager.SetLocalMute(true);
    }

    public void StopPerform()
    {
        StudioStreamRequest req = new StudioStreamRequest();
        req.streamKey = apiManager.verseInfo.streamKey;
        deviceChannel.Emit("reqStreamEnded", req);
        networkManager.root.Emit("perform", false);
        uIManager.reactionBanner.gameObject.SetActive(false);
        uIManager.SetStageMood(false, networkManager.socketId);
        rtcManager.SetLocalMute(false);
    }

    void OnError()
    {
        Debug.LogError("Error occured");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
