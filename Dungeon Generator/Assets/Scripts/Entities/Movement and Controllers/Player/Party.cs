using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Party : MonoBehaviour
{
    public static Party instance;
    List<PlayableCharacter> m_PartyMembers;
    [SerializeField]PlayableCharacter m_PartyLeader;

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
            if(CameraMovement.cameraConstraints != Vector2.zero)
            {
                if(cameraRotationObject.transform.position.x >= CameraMovement.cameraConstraints.x)
                {
                    cameraRotationObject.transform.position = new Vector3(cameraRotationObject.transform.position.x - (cameraRotationObject.transform.position.x - CameraMovement.cameraConstraints.x) - 0.5f, cameraRotationObject.transform.position.y, cameraRotationObject.transform.position.z);
                }
                else if(cameraRotationObject.transform.position.x < CameraMovement.cameraConstraints.x)
                {
                    cameraRotationObject.transform.position = new Vector3(cameraRotationObject.transform.position.x + ( CameraMovement.cameraConstraints.x - cameraRotationObject.transform.position.x) - 0.5f, cameraRotationObject.transform.position.y, cameraRotationObject.transform.position.z);
                }
                if(cameraRotationObject.transform.position.y >= CameraMovement.cameraConstraints.y)
                {
                    cameraRotationObject.transform.position = new Vector3(cameraRotationObject.transform.position.x, cameraRotationObject.transform.position.y - (cameraRotationObject.transform.position.y - CameraMovement.cameraConstraints.y) - 0.5f, cameraRotationObject.transform.position.z);
                }
                else if(cameraRotationObject.transform.position.y < CameraMovement.cameraConstraints.y)
                {
                    cameraRotationObject.transform.position = new Vector3(cameraRotationObject.transform.position.x, cameraRotationObject.transform.position.y + ( CameraMovement.cameraConstraints.y - cameraRotationObject.transform.position.y) - 0.5f, cameraRotationObject.transform.position.z);
                }
            }
        }
    }
    public bool LerpCamera(Vector3 newPosition)
    {
        cameraRotationObject.transform.position = new Vector3(Mathf.Lerp(cameraRotationObject.transform.position.x, newPosition.x, movementSpeed), Mathf.Lerp(cameraRotationObject.transform.position.y, newPosition.y, movementSpeed), newPosition.z);
        if((int)(cameraRotationObject.transform.position.x *1.75f) == (int)(newPosition.x *1.75f) && (int)cameraRotationObject.transform.position.y == (int)newPosition.y)
        {
            cameraRotationObject.transform.position = newPosition;
            m_PartyLeader.GetPMM().canMove = true;
            movingRoom = false;
            return true;
        }
        return false;
    }
}
