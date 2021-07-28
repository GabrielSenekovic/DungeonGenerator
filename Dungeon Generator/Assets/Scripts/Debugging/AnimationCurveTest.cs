using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCurveTest : MonoBehaviour
{
    public AnimationCurve curve;
    void Start()
    {
        Keyframe temp = new Keyframe();
        temp.time = 0;
        temp.value = 0;
        temp.inTangent = 1;
        curve.AddKey(temp);

        temp.time = 0.5f;
        temp.value = 0.25f;
        temp.inTangent = 0;
        temp.outTangent = 0;

        curve.AddKey(temp);

        temp.time = 1;
        temp.value = 0;
        curve.AddKey(temp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
