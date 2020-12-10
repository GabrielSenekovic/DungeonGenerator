using UnityEngine;
using System.Collections;

public class PlayerInteractionModel : MonoBehaviour
{
    Collider2D m_Collider;
    [SerializeField]InteractableBase m_Interactable;

    public void Initialize(Collider2D collider)
    {
        m_Collider = collider;
    }

    public void OnInteract()
    {
        if (m_Interactable != null)
        {
            m_Interactable.OnInteract();
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<InteractableBase>() && collision.collider.GetComponent<InteractableBase>().GetIsInteractable() == true)
        {
            m_Interactable = collision.collider.GetComponent<InteractableBase>();
        }     
    }
    public void OnCollisionExit(Collision collision)
    {
        if (collision.collider.GetComponent<InteractableBase>())
        {
            if (m_Interactable != null)
            {
                m_Interactable = null;
            }
        }
    }
}
