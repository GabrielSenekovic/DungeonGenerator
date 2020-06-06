using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    //This controller will control whoever is the party leader
    Party m_Party;

    public void Awake()
    {
        m_Party = GetComponent<Party>();
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                Move(KeyCode.W);
            }
            if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
            {
                Move(KeyCode.S);
            }
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                Move(KeyCode.A);
            }
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                Move(KeyCode.D);
            }
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }
    }

    public void Move(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.W:
                m_Party.GetPartyLeader().GetPMM().SetDirection(new Vector2(0, 1));
                break;
            case KeyCode.A:
                m_Party.GetPartyLeader().GetPMM().SetDirection(new Vector2(-1, 0));
                break;
            case KeyCode.S:
                m_Party.GetPartyLeader().GetPMM().SetDirection(new Vector2(0, -1));
                break;
            case KeyCode.D:
                m_Party.GetPartyLeader().GetPMM().SetDirection(new Vector2(1, 0));
                break;
            default:
                break;
        }
    }
    public void Interact()
    {
        m_Party.GetPartyLeader().GetPIM().OnInteract();
    }
}
