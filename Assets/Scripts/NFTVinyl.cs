using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class NFTVinyl : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void OpenPage(string url);

    public int tokenId;

    public void SetVinyl(int id)
    {
        tokenId = id;
        gameObject.SetActive(id > 0);
    }

    void OnMouseDown()
    {
        OpenPage($"https://opensea.io/assets/matic/0x55c1e448843325b91a3d90745fb2c781400b93b9/{tokenId}");
    }

    void OnMouseOver()
    {

    }
}
