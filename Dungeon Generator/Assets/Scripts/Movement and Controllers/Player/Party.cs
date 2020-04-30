using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Party : MonoBehaviour
{
    List<Character> m_PartyMembers;
    [SerializeField]Character m_PartyLeader;

    public void ChangePartyLeader(int index)
    {
        m_PartyLeader = m_PartyMembers[index];
    }

    public Character GetPartyLeader()
    {
        return m_PartyLeader;
    }
}
