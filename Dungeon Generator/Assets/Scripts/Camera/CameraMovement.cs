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
    public enum CameraMovementMode
    {
        SingleRoom = 0,
        Free = 1
    }
    float rotationSideways = 0;
    public int rotationSpeed;
    [SerializeField]Party party;
    public CameraMode mode = CameraMode.Side;
    public static CameraMovementMode movementMode = CameraMovementMode.Free;
    public static Vector2 cameraConstraints;
    public Vector2 cameraConstraints2;

    void Awake()
    {
        transform.eulerAngles = new Vector3(-45, transform.rotation.y, transform.rotation.z);
        transform.position = new Vector3(transform.position.x, -11, -8.2f);
    }
    void LateUpdate()
    {
        cameraConstraints2 = cameraConstraints;
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            Rotate(rotationSpeed);
            //rotatedPosition = new Vector3(temp.x - transform.position.x, temp.y - transform.position.y, temp.z - transform.position.z);
            rotationSideways+= rotationSpeed;
        }
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            Rotate(-rotationSpeed);
            rotationSideways-= rotationSpeed;
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            //ToggleCameraMode();
        }
        //transform.position = new Vector3(party.GetPartyLeader().transform.position.x + rotatedPosition.x, party.GetPartyLeader().transform.position.y + rotatedPosition.y,party.GetPartyLeader().transform.position.z + rotatedPosition.z);
    }
    void Rotate(float speed)
    {
        transform.RotateAround(new Vector3(party.cameraRotationObject.transform.position.x, party.cameraRotationObject.transform.position.y, 0), Vector3.forward, speed);
        party.GetPartyLeader().GetComponentInChildren<SpriteRenderer>().transform.RotateAround(party.GetPartyLeader().transform.position, Vector3.forward, speed);
    }
    void ToggleCameraMode()
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
    void ZoomInOut(float value)
    {
        //transform.position = new Vector3(transform.position.x + rotatedPosition.x, transform.position.y + rotatedPosition.y, transform.position.z + value);
    }
}
