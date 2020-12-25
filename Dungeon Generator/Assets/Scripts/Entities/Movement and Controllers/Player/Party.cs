using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Party : MonoBehaviour
{
    public static Party instance;
    List<PlayableCharacter> m_PartyMembers;
    [SerializeField]PlayableCharacter m_PartyLeader;
    public float cameraSpeed;

    public bool movingRoom = false;

    float movementSpeed = 0.1f;

    public GameObject cameraRotationObject;

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
    private void Update() 
    {
        if(!movingRoom)
        {
            cameraRotationObject.transform.position = m_PartyLeader.transform.position;

            Vector2 anchor = CameraMovement.cameraAnchor_in; //Abbreviated for readability

            if(CameraMovement.cameraAnchor_in != Vector2.zero) //Do not move the cmaera object if there is an anchor
            {cameraRotationObject.transform.position = anchor;}
        }
    }
    public bool MoveCamera(Vector3 newPosition)
    {
        Vector2 direction = (newPosition - cameraRotationObject.transform.position).normalized;

        cameraRotationObject.transform.position = new Vector3(cameraRotationObject.transform.position.x + direction.x * cameraSpeed, cameraRotationObject.transform.position.y + direction.y * cameraSpeed, newPosition.z);

        if((int)cameraRotationObject.transform.position.x == (int)newPosition.x && (int)cameraRotationObject.transform.position.y == (int)newPosition.y)
        {
            cameraRotationObject.transform.position = newPosition;
            m_PartyLeader.GetPMM().canMove = true;
            movingRoom = false;
            return true;
        }
        return false;
    }
}
