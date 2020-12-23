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

            Vector2 c_constraints = CameraMovement.cameraConstraints; //Abbreviated for readability
            Vector3 rot_pos = cameraRotationObject.transform.position;

            if(CameraMovement.cameraConstraints != Vector2.zero) 
            //Only move the point around which the camera rotates if there are constraints to the room
            {
                cameraRotationObject.transform.position = rot_pos.x >= c_constraints.x 
                    ? new Vector3(c_constraints.x - 0.5f, rot_pos.y, rot_pos.z)
                    : new Vector3(rot_pos.x + (c_constraints.x - rot_pos.x) - 0.5f, rot_pos.y, rot_pos.z);

                cameraRotationObject.transform.position = cameraRotationObject.transform.position.y >= c_constraints.y
                    ? new Vector3(rot_pos.x, c_constraints.y - 0.5f, rot_pos.z)
                    : new Vector3(rot_pos.x, rot_pos.y + (c_constraints.y - rot_pos.y) - 0.5f, rot_pos.z);
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
