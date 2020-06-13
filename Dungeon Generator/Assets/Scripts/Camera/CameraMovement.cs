using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    int rotationSideways = 0;

    void Awake()
    {
        transform.eulerAngles = new Vector3(-45, transform.rotation.y, transform.rotation.z);
        transform.position = new Vector3(transform.position.x, -1.3f, -8.2f);
    }
    void LateUpdate()
    {
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.RotateAround(new Vector3(9.5f, 9.5f, 0), Vector3.forward, 1);
            rotationSideways++;
        }
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.RotateAround(new Vector3(9.5f, 9.5f, 0), Vector3.forward, -1);
            rotationSideways--;
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            if(transform.position.z == -19)
            {
                transform.eulerAngles = new Vector3(-45, transform.rotation.y, transform.rotation.z);
                transform.position = new Vector3(transform.position.x, -1.3f, -8.2f);
                transform.RotateAround(new Vector3(9.5f, 9.5f, 0), Vector3.forward, rotationSideways);
            }
            else
            {
                transform.position = new Vector3(9.5f, 9.5f, -19);
                transform.eulerAngles = new Vector3(0, 0, rotationSideways);
            }
        }
    }
    void ZoomInOut(float value)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + value);
    }
}
