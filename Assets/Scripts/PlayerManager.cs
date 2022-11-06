using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketDTO;

public class PlayerManager : MonoBehaviour
{
    public int sheetCount;
    public GameObject playerPrefab;
    public int playerId = 0;

    public CharacterLook me;
    CharacterReaction reactor;
    public Dictionary<string, Transform> others = new Dictionary<string, Transform>();

    public bool isSignedIn = false;

    private NetworkManager networkManager;
    private UIManager uiManager;

    void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
        uiManager = GetComponent<UIManager>();
        me.SpriteSheetName = string.Format("chara_{0:D2}", Random.Range(1, sheetCount + 1));
        reactor = me.GetComponent<CharacterReaction>();
    }

    public void OnPlayerJoin(JoinResponse data)
    {
        Debug.Log($"{data.nickname} Joined");
        GameObject newPlayer = GameObject.Instantiate(playerPrefab);
        newPlayer.GetComponentInChildren<CharacterLook>().SpriteSheetName = data.spriteName;
        newPlayer.GetComponentInChildren<CharacterLook>().nameLabel.text = data.nickname;
        others[data.id] = newPlayer.transform.GetChild(0);
        others[data.id].position = new Vector2(data.pos[0], data.pos[1]);
        others[data.id].GetComponentInChildren<CharacterLook>().vinyl.SetVinyl(data.tokenId);
    }

    public void OnPlayerVinyl(VinylResponse data)
    {
        if (others.ContainsKey(data.id))
        {
            others[data.id].GetComponentInChildren<CharacterLook>().vinyl.SetVinyl(data.tokenId);
        }
    }

    public void OnPlayerLeave(LeaveResponse data)
    {
        if (others.ContainsKey(data.id))
        {
            GameObject.Destroy(others[data.id].gameObject);
            others.Remove(data.id);
        }
    }

    public void OnPlayerTransform(TransformResponse data)
    {
        if (others.ContainsKey(data.id))
        {
            others[data.id].position = new Vector2(data.pos[0], data.pos[1]);
        }
    }

    public void OnPlayerReaction(ReactionResponse data)
    {
        if (others.ContainsKey(data.id))
        {
            others[data.id].GetComponent<CharacterReaction>().React(data.idx);
            uiManager.reactionBanner.FeedReaction(data.idx);
        }
    }

    public void SetSpotLight(string id, bool on)
    {
        if (id == networkManager.socketId)
        {
            me.spotLight.intensity = on ? 0.8f : 0f;
        }
        else if (others.ContainsKey(id))
        {
            others[id].GetComponent<CharacterLook>().spotLight.intensity = on ? 0.8f : 0f;
        }
    }

    void SendReaction(int idx)
    {
        ReactionRequest req = new ReactionRequest();
        uiManager.reactionBanner.FeedReaction(idx);
        req.idx = idx;
        networkManager.root.Emit("reaction", req);
    }

    public void OnMemberRefresh(MembersResponse data)
    {
        Debug.Log("Member Refresh");
        foreach (JoinResponse member in data.members)
        {
            if (member.id != data.id)
            {
                OnPlayerJoin(member);
            }
        }
    }

    public void OnUpdateName(NameResponse data)
    {
        if (others.ContainsKey(data.id))
        {
            others[data.id].GetComponent<CharacterLook>().nameLabel.text = data.name;
        }
    }

    public void JoinRoom(int id)
    {
        JoinRequest request = new JoinRequest();
        request.nickname = "";
        request.spriteName = me.SpriteSheetName;
        request.pos = new float[2] { me.transform.position.x, me.transform.position.y };
        networkManager.root.Emit("join", id, request);
    }

    // Update is called once per frame
    void Update()
    {
        if (me.movement.magnitude > 0.01f)
        {
            TransformRequest td = new TransformRequest();
            td.pos = new float[2] { me.transform.position.x, me.transform.position.y };
            networkManager.root.Emit("transform", td);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            reactor.React(0);
            SendReaction(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            reactor.React(1);
            SendReaction(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            reactor.React(2);
            SendReaction(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            reactor.React(3);
            SendReaction(3);
        }
    }
}
