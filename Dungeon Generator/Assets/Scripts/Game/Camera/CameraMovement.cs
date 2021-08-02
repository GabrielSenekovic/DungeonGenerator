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

    static CameraMovement instance;
    public static CameraMovement Instance
    {
        get
        {
            return instance;
        }
    }
    public static float rotationSideways = 0;
    public int rotationSpeed;
    [SerializeField]Party party;
    public CameraMode mode = CameraMode.Side;
    public static CameraMovementMode movementMode = CameraMovementMode.Free;
    public Vector2 cameraAnchor;

    bool movingRoom = false;
    public GameObject cameraRotationObject;
    public float transitionSpeed;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        transform.eulerAngles = new Vector3(-53, transform.rotation.y, transform.rotation.z);
        transform.position = new Vector3(transform.position.x, -15, -10); //-11, -8.2f
    }
    private void Update() 
    {
        if(!movingRoom)
        {
            //cameraRotationObject.transform.position = m_PartyLeader.transform.position;

            //if(CameraMovement.cameraAnchor_in != Vector2.zero) //Do not move the cmaera object if there is an anchor
            //{
                cameraRotationObject.transform.position = cameraAnchor;
                //}
        }
    }
    void LateUpdate()
    {
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            Rotate(rotationSpeed);
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
    }
    void Rotate(float speed)
    {
        transform.RotateAround(new Vector3(cameraRotationObject.transform.position.x, cameraRotationObject.transform.position.y, 0), Vector3.forward, speed);
        VisualsRotator.RotateAll(speed);
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
    public bool MoveCamera(Vector3 newPosition, Vector3 currentPosition)
    {
        cameraRotationObject.transform.position = Math.Transition(cameraRotationObject.transform.position, newPosition, currentPosition, transitionSpeed);

        if(cameraRotationObject.transform.position == newPosition)
        {
            Party.instance.GetPartyLeader().GetPMM().SetCanMove(true);
            movingRoom = false;
            return true;
        }
        return false;
    }
    public static void SetCameraAnchor(Vector2 anchor_in)
    {
        instance.cameraAnchor = anchor_in;
    }
    public static void SetMovingRoom(bool value)
    {
        instance.movingRoom = value;
    }
    public static bool GetMovingRoom()
    {
        return instance.movingRoom;
    }
    public static GameObject GetRotationObject()
    {
        return instance.cameraRotationObject;
    }
}
