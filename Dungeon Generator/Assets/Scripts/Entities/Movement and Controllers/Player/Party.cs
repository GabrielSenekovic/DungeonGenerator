using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Party : MonoBehaviour
{
    public static Party instance;
    List<PlayableCharacter> m_PartyMembers;
    [SerializeField]PlayableCharacter m_PartyLeader;

    float movementSpeed = 0.1f;

    private void Awake() 
    {
        instance = this;
    }

    public void ChangePartyLeader(int index)
    {
        m_PartyLeader = m_PartyMembers[index];
    }

    public PlayableCharacter GetPartyLeader()
    {
        return m_PartyLeader;
    }
}
