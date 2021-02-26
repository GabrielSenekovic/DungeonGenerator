using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Math : MonoBehaviour
{
    //1/limit * x^exponent / limit^exponent
    //The x^exponent determines the curve
    //The limit^exponent only brings it down so that when x is limit, y is one
    // float s = interval * Mathf.Pow(i, exponent) / Mathf.Pow(6, exponent-1) *-1 +1;
    public static float SemiCircle(float x, float limit, float curve)
    {
        return Mathf.Sqrt(Mathf.Pow(limit, curve) - Mathf.Pow(x, curve)) / limit;
    }
    public static float SemiSuperEllipse(float x, float interval, float limit, float curve)
    {
        return 1 - interval * Mathf.Pow(x, curve) / Mathf.Pow(limit, curve - 1);
    }
    public static float Mod(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
    }
    public static bool Compare(float a, float b)
    {
        if(a < 0)
        {
            return Mathf.FloorToInt(a) == Mathf.FloorToInt(b);
        }
        else if(a >= 0)
        {
            return (int)a == (int)b;
        }
        return false;
    }
}
