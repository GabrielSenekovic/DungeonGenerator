using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Party : MonoBehaviour
{
    List<PlayableCharacter> m_PartyMembers;
    [SerializeField]PlayableCharacter m_PartyLeader;

    public void ChangePartyLeader(int index)
    {
        m_PartyLeader = m_PartyMembers[index];
    }

    public PlayableCharacter GetPartyLeader()
    {
        return m_PartyLeader;
    }
}
