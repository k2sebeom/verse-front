using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    private bool isInteractable = false;

    public KeyCode key;
    public UnityEvent onInteract;

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            if (isInteractable)
            {
                onInteract.Invoke();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        isInteractable = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        isInteractable = false;
    }
}
