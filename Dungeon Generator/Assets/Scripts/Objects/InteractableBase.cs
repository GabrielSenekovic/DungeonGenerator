using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBase : MonoBehaviour
{
    protected bool isInteractable = true;

    public virtual void OnInteract()
    {

    }

    public bool GetIsInteractable()
    {
        return isInteractable;
    }
}
