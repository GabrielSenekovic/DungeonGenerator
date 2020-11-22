﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    //This controller will control whoever is the party leader
    Party party;

    [SerializeField] GameObject camera;

    public void Awake()
    {
        party = GetComponent<Party>();
    }

    void Start() 
    {
        VisualsRotator.renderers.Add(GetComponentInChildren<SpriteRenderer>().gameObject);
    }
    public void Update()
    {
        Move();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            UIManager.OpenOrClose(UIManager.UIScreen.MainMenu);
            UIManager.ToggleHUD();
        }
    }

    void Move()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if(party.GetPartyLeader().GetComponent<StatusConditionModel>().rigid)
            {
                if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
                {
                    OnMove(KeyCode.W);
                }
                if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
                {
                    OnMove(KeyCode.S);
                }
                if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                {
                    OnMove(KeyCode.A);
                }
                if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
                {
                    OnMove(KeyCode.D);
                }
            }
            else
            {
                Vector2 temp = Vector2.zero;
                if(Input.GetKey(KeyCode.A)) { temp.x =-1;}
                if(Input.GetKey(KeyCode.D)) { temp.x = 1;}
                if(Input.GetKey(KeyCode.W)) { temp.y = 1;}
                if(Input.GetKey(KeyCode.S)) { temp.y =-1;}

                party.GetPartyLeader().GetPMM().Dir = Quaternion.Euler(0, 0, camera.transform.rotation.eulerAngles.z) * temp;
                party.GetPartyLeader().GetPMM().facingDirection = party.GetPartyLeader().GetPMM().Dir;
            }
            party.GetPartyLeader().GetPMM().currentSpeed = party.GetPartyLeader().GetPMM().speed;
        }
        else
        {
            party.GetPartyLeader().GetPMM().Vel = Vector2.zero;
        }
    }
    public void OnMove(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.W:
                party.GetPartyLeader().GetPMM().Dir = new Vector2(0, 1);
                break;
            case KeyCode.A:
                party.GetPartyLeader().GetPMM().Dir = new Vector2(-1, 0);
                break;
            case KeyCode.S:
                party.GetPartyLeader().GetPMM().Dir = new Vector2(0, -1);
                break;
            case KeyCode.D:
                party.GetPartyLeader().GetPMM().Dir = new Vector2(1, 0);
                break;
            default:
                break;
        }
        party.GetPartyLeader().GetPMM().facingDirection = party.GetPartyLeader().GetPMM().Dir;
    }
    public void Interact()
    {
        party.GetPartyLeader().GetPIM().OnInteract();
    }
}
