using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawnDebug : MonoBehaviour
{
    [System.Serializable]public struct WaveData
    {
        public int horizontalAmount;
        public float radius;
        [Range(0.0f, 1.0f)]
        public float curvature;
        public float separation;
    }
    public List<WaveData> waveData;
    int waveIndex = 0;

    public ProjectileAttackIdentifier attack;

    public int attackFrequency_Limit;
    int attackFrequency_Timer = 0;

    public int arrowLength;

    private void OnDrawGizmos()
    {
        WaveData w = waveData[waveIndex];
        Gizmos.color = Color.green;
        List < Vector2 > points = new List<Vector2>();
        float displacement = 0;
        float angle = (180 + 90 / 2) * Mathf.Deg2Rad; //Sets it so the first point goes forward
        float angleIncrease = 360 / w.horizontalAmount * Mathf.Deg2Rad * w.curvature;

        for (int i = 0; i < w.horizontalAmount; i++)
        {
            float c_Angle = angle - angleIncrease * w.horizontalAmount / 2 + angleIncrease / 2; 
            float xCurve = w.radius * (Mathf.Cos(c_Angle) - Mathf.Sin(c_Angle));
            float yCurve = w.radius * (Mathf.Sin(c_Angle) + Mathf.Cos(c_Angle));

            Gizmos.color = Gizmos.color == Color.green ? Color.blue : Color.green;

            float x_f = xCurve + w.separation * i - w.separation * w.horizontalAmount / 2 + w.separation / 2;
            float x_t = xCurve + w.separation * i - w.separation * w.horizontalAmount / 2 + w.separation / 2;

            Vector2 from = new Vector2(x_f, yCurve);
            Vector2 to = new Vector2(x_t, -1 * arrowLength - waveData[waveIndex].radius);

            Gizmos.DrawLine(from, to);
            Gizmos.DrawLine(to, new Vector2(to.x - 0.2f, to.y + 0.5f));
            Gizmos.DrawLine(to, new Vector2(to.x + 0.2f, to.y + 0.5f));
            displacement++; angle += angleIncrease;
            points.Add(from);
        }
        for(int i = 0; i < points.Count - 1; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }

    private void FixedUpdate()
    {
        attackFrequency_Timer++;
        if (attackFrequency_Timer >= attackFrequency_Limit)
        {
            attackFrequency_Timer = 0;
            float displacement = 0;
            for (int i = 0; i < waveData[waveIndex].horizontalAmount; i++)
            {
                Attack(new Vector2(0, -1), new Vector2(displacement - (float)(waveData[waveIndex].horizontalAmount - 1) / 2, -waveData[waveIndex].radius));
                displacement++;
            }
            waveIndex++; waveIndex %= waveData.Count;
        }
    }

    public void Attack(Vector2 direction)
    {
        attack.Attack(direction, transform.position, GetComponent<Collider>());
    }
    public void Attack(Vector2 direction, Vector2 displacement)
    {
        attack.Attack(direction, (Vector2)transform.position + displacement, GetComponent<Collider>());
    }
}
