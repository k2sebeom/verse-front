using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReactionBanner : MonoBehaviour
{
    public GameObject[] reactions;
    public int[] counts = new int[4] { 0, 0, 0, 0 };

    public void Initialize()
    {
        counts = new int[4] { 0, 0, 0, 0 };
        foreach (GameObject r in reactions)
        {
            r.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            r.GetComponentInChildren<TMP_Text>().text = "0";
        }
    }

    void Awake()
    {
        Initialize();
    }

    public void FeedReaction(int idx)
    {
        RectTransform rect = reactions[idx].GetComponent<RectTransform>();
        rect.localScale = rect.localScale + new Vector3(0.01f, 0.01f, 0f);
        counts[idx] += 1;
        reactions[idx].GetComponentInChildren<TMP_Text>().text = $"{counts[idx]}";
    }
}
