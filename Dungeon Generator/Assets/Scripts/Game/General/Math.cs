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
    public static int[] GetValidConstraints(int i, int range, Vector2Int grid)
    {
        int targY = (i / grid.x);
        int startY = (targY - range) % grid.y; //! POS - 1 
        int targX = i % grid.x;
        if (startY < 0) { startY = 0; }
        int startX = (targX - range) % grid.x; //! POS - 1
        if (startX < 0) { startX = 0; }
        int yLimit = targY + range + 1;
        int xLimit = targX + range + 1;

        if (xLimit > grid.x) { xLimit = grid.x; }
        if (yLimit > grid.y) { yLimit = grid.y; }

        return new int[4]{ startX, startY, xLimit, yLimit };
    }
}
