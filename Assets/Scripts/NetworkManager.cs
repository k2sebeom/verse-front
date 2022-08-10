using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.SocketIO3;
using SocketDTO;

public class NetworkManager : MonoBehaviour
{
    private SocketManager manager;
    public Socket root;

    public string socketId;

    public NetworkSetting setting;

    private PlayerManager playerManager;
    private UIManager uiManager;

    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        uiManager = GetComponent<UIManager>();
    }

    // Start is called before the first frame update
    public void Connect()
    {
        manager = new SocketManager(new System.Uri(setting.BASEURL));

        root = manager.Socket;

        root.On(SocketIOEventTypes.Connect, OnConnect);

        root.On<string>("id", OnIdentity);

        root.On<JoinResponse>("join", playerManager.OnPlayerJoin);

        root.On<LeaveResponse>("leave", playerManager.OnPlayerLeave);

        root.On<TransformResponse>("transform", playerManager.OnPlayerTransform);

        root.On<MembersResponse>("members", playerManager.OnMemberRefresh);

        root.On<NameResponse>("name", playerManager.OnUpdateName);

        root.On<PerformResponse>("perform", uiManager.OnPerform);

        root.On<VinylResponse>("vinyl", playerManager.OnPlayerVinyl);

        root.On<ReactionResponse>("reaction", playerManager.OnPlayerReaction);
    }

    void OnIdentity(string id)
    {
        if (socketId.Length > 0)
        {
            root.Emit("leave", socketId);
        }
        socketId = id;
    }

    void OnConnect()
    {
        playerManager.JoinRoom(uiManager.roomId);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
