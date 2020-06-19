using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualsRotator : MonoBehaviour
{
    public static List<GameObject> renderers = new List<GameObject>(){};
    void LateUpdate()
    {
        for(int i = 0; i < renderers.Count; i++)
        {
            try
            {
                renderers[i].transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, renderers[i].transform.rotation.eulerAngles.z);
            }
            catch
            {
                renderers.RemoveAt(i);
                i--;
            }
        }
    }
    public static void RotateAll(float speed)
    {
        for(int i = 0; i < renderers.Count; i++)
        {
            try
            {
                renderers[i].transform.RotateAround(renderers[i].transform.position, Vector3.forward, speed);
            }
            catch
            {
                renderers.RemoveAt(i);
                i--;
            }
        }
    }
}
