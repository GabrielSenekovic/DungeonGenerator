using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public enum CameraMode
    {
        TopDown = 0,
        Side = 1
    }
    int rotationSideways = 0;
    [SerializeField]Party party;
    public CameraMode mode = CameraMode.Side;

    void Awake()
    {
        transform.eulerAngles = new Vector3(-45, transform.rotation.y, transform.rotation.z);
        transform.position = new Vector3(transform.position.x, -8.5f, -8.2f);
    }
    void LateUpdate()
    {
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.RotateAround(new Vector3(party.GetPartyLeader().transform.position.x, party.GetPartyLeader().transform.position.y, 0), Vector3.forward, 1);
            party.GetPartyLeader().GetComponentInChildren<SpriteRenderer>().transform.RotateAround(party.GetPartyLeader().transform.position, Vector3.forward, 1);
            rotationSideways++;
        }
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.RotateAround(new Vector3(party.GetPartyLeader().transform.position.x, party.GetPartyLeader().transform.position.y, 0), Vector3.forward, -1);
            party.GetPartyLeader().GetComponentInChildren<SpriteRenderer>().transform.RotateAround(party.GetPartyLeader().transform.position, Vector3.forward, -1);
            rotationSideways--;
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            if(transform.position.z == -19)
            {
                transform.eulerAngles = new Vector3(-45, transform.rotation.y, transform.rotation.z);
                transform.position = new Vector3(transform.position.x, -8.5f, -8.2f);
                transform.RotateAround(new Vector3(party.GetPartyLeader().transform.position.x, party.GetPartyLeader().transform.position.y, 0), Vector3.forward, rotationSideways);
                mode = CameraMode.Side;
            }
            else
            {
                transform.position = new Vector3(party.GetPartyLeader().transform.position.x, party.GetPartyLeader().transform.position.y, -19);
                transform.eulerAngles = new Vector3(0, 0, rotationSideways);
                mode = CameraMode.TopDown;
            }
        }
    }
    void ZoomInOut(float value)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + value);
    }
}
