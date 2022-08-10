using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using TMPro;
using APIDTO;
using SocketDTO;
using UnityEngine.Rendering.Universal;

public class UIManager : MonoBehaviour
{
    public GameObject signInModal;
    public TMP_InputField phoneField;
    public TMP_InputField codeField;

    public GameObject performButton;

    public TMP_Text roomNameLabel;

    public ReactionBanner reactionBanner;

    public GameObject signOutButton;

    public GameObject vinylModal;
    public TMP_InputField tokenIdField;

    public Light2D globalLight;

    public int roomId = -1;

    public bool isPerforming = false;

    public GetMusicianResponse roomInfo;

    public GameObject stageBlock;
    public GameObject backStageBlock;

    private PlayerManager playerManager;
    private NetworkManager networkManager;
    private APIManager apiManager;
    private StudioManager studioManager;

    private int queryTokenId;

    [DllImport("__Internal")]
    private static extern int GetRoomId();

    [DllImport("__Internal")]
    private static extern void PlayUrl(string url);

    [DllImport("__Internal")]
    private static extern void OpenMetaMask();

    [DllImport("__Internal")]
    private static extern void CheckNFTOwnership(int id);

    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        apiManager = GetComponent<APIManager>();
        networkManager = GetComponent<NetworkManager>();
        studioManager = GetComponent<StudioManager>();
    }

    void Start()
    {
        try
        {
            roomId = GetRoomId();
        }
        catch
        {
            roomId = -1;
        }

        networkManager.Connect();
        if (roomId > 0)
        {
            StartCoroutine(apiManager.GetMusician(roomId));
        }
        if (PlayerPrefs.HasKey("cred"))
        {
            StartCoroutine(apiManager.AutoSignIn());
        }
    }

    public void OnMusicianInfo(GetMusicianResponse response)
    {
        roomInfo = response;
        roomNameLabel.text = $"{response.alias}'s Room";
    }

    public void OpenSignInModal()
    {
        if (!playerManager.isSignedIn)
        {
            playerManager.me.GetComponent<PlayerMovement>().enabled = false;
            signInModal.SetActive(true);
        }
    }

    public void OpenWallet()
    {
        OpenMetaMask();
    }

    public void CheckNFT()
    {
        queryTokenId = System.Convert.ToInt32(tokenIdField.text);
        CheckNFTOwnership(queryTokenId);
    }

    public void HandleNFTResult(int result)
    {
        Debug.Log($"NFT ownership {result}");
        if (result > 0)
        {
            SetMyVinyl(queryTokenId);
            vinylModal.SetActive(false);
        }
        else
        {
            tokenIdField.text = $"Token {queryTokenId} is not yours";
        }
        // TODO This should not be here
        SetMyVinyl(queryTokenId);
    }

    public void SetMyVinyl(int tokenId)
    {
        playerManager.me.vinyl.SetVinyl(tokenId);
        VinylRequest req = new VinylRequest();
        req.tokenId = tokenId;
        networkManager.root.Emit("vinyl", req);
    }

    public void CloseSignInModal()
    {
        playerManager.me.GetComponent<PlayerMovement>().enabled = true;
        signInModal.SetActive(false);
    }

    public void SendCode()
    {
        StartCoroutine(apiManager.SendCode(phoneField.text));
    }

    public void SignIn()
    {
        StartCoroutine(apiManager.SignIn(phoneField.text, codeField.text));
    }

    public void SignOut()
    {
        PlayerPrefs.DeleteKey("cred");
        apiManager.RefreshPage();
    }

    public void SetPerformButton(bool active)
    {
        performButton.SetActive(active);
    }

    public void OnSignInResponse(SignInResponse response)
    {
        CloseSignInModal();
        playerManager.isSignedIn = true;
        playerManager.me.nameLabel.text = response.userAlias;
        networkManager.root.Emit("name", response.userAlias);
        playerManager.playerId = response.id;

        signOutButton.SetActive(true);

        if (playerManager.playerId == roomId)
        {
            studioManager.Connect();
            stageBlock.SetActive(false);
            backStageBlock.SetActive(false);
        }
    }

    public void OnPerform(PerformResponse data)
    {
        isPerforming = data.state;
        SetStageMood(data.state, data.id);
        if (data.state && playerManager.isSignedIn)
        {
            reactionBanner.Initialize();
            reactionBanner.gameObject.SetActive(true);
            PlayUrl(roomInfo.liveUrl);
        }
        else if (!data.state)
        {
            reactionBanner.gameObject.SetActive(false);
        }
    }

    public void SetStageMood(bool state, string performerId)
    {
        if (state)
        {
            StartCoroutine(Fade(1f, 0.3f));
            playerManager.SetSpotLight(performerId, true);
        }
        else
        {
            StartCoroutine(Fade(0.3f, 1f));
            playerManager.SetSpotLight(performerId, false);
        }
    }

    private IEnumerator Fade(float start, float end)
    {
        float step = (end - start) / 60;
        globalLight.intensity = start;
        for (int i = 0; i < 60; i++)
        {
            globalLight.intensity = start + (step * i);
            yield return new WaitForSeconds(1f / 60f);
        }
        globalLight.intensity = end;
    }

    public void ToNextMusician()
    {
        StartCoroutine(apiManager.GetRandomMusician());
    }
}
