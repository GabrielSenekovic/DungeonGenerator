using UnityEngine;
using System.Collections;

public enum WallVariant
{
    None = 0,
    Bottom = 1,
    BottomLeft = 2,
    BottomRight = 3,
    Side = 4,
    TopCorner = 5
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
