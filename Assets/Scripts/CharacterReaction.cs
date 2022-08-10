using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterReaction : MonoBehaviour
{
    public GameObject reactionBubble;

    public GameObject[] reactions;

    public void React(int idx)
    {
        TurnOffReaction();
        CancelInvoke("TurnOffReaction");
        reactionBubble.SetActive(true);
        reactions[idx].SetActive(true);
        Invoke("TurnOffReaction", 1);
    }

    void TurnOffReaction()
    {
        reactionBubble.SetActive(false);
        foreach (GameObject r in reactions)
        {
            r.SetActive(false);
        }
    }
}
