using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : InteractableBase
{
    public override void OnInteract()
    {
        if (isInteractable)
        {
            isInteractedWith = true;
            //Usually talk
        }
    }
}
