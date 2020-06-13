using UnityEngine;
using System.Collections;

public enum WallVariant
{
    None = 0,
    Side = 1,
    Corner = 2,
    Column = 3,
    T = 4,
    Cross = 5,
    End = 6
}

public class WallPosition : MonoBehaviour
{
    WallVariant m_variant = WallVariant.None;

    bool m_IsOccupied = false;

    public void PlaceDown()
    {
        m_IsOccupied = true;
    }
    public void UnPlace()
    {
        m_IsOccupied = false;
    }
    public bool GetIsOccupied()
    {
        return m_IsOccupied;
    }
    public void SetVariant(WallVariant variant)
    {
        m_variant = variant;
    }
    public WallVariant GetVariant()
    {
        return m_variant;
    }
}
