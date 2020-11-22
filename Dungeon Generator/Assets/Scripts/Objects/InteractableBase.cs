using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBase : MonoBehaviour
{
    protected bool isInteractable = true;
    public bool isInteractedWith = false;

    public virtual void OnInteract()
    {
        isInteractedWith = true;
    }

    public bool GetIsInteractable()
    {
        return isInteractable;
    }
}
