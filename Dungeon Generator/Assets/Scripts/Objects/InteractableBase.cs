using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBase : MonoBehaviour
{
    bool m_IsInteractable = true;

    public virtual void OnInteract()
    {

    }

    public bool GetIsInteractable()
    {
        return m_IsInteractable;
    }

    public void SetIsInteractable(bool value)
    {
        m_IsInteractable = value;
    }
}
