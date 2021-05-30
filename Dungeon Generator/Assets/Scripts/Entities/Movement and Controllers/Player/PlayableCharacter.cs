using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacter : MonoBehaviour
{
    PlayerData m_data;

    [SerializeField] Collider2D m_collider;

    MovementModel m_PMM;
    PlayerInteractionModel m_PIM;

    private void Awake()
    {
        m_PMM = GetComponent<MovementModel>();
        m_PIM = GetComponent<PlayerInteractionModel>();
    }

    private void Start()
    {
        m_PIM.Initialize(m_collider);
    }

    public MovementModel GetPMM()
    {
        return m_PMM;
    }
    public PlayerInteractionModel GetPIM()
    {
        return m_PIM;
    }
}
