using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProjectileSpawnDebug : MonoBehaviour
{
    [System.Serializable]public struct WaveData
    {
        public Vector2 position;

        public int amountOfProjectiles;
        public float radius;
        [Range(0.0f, 1.0f)]
        public float curvature;
        public float separation;

        public Vector2 globalDirection;
        public float projectileSpread;

        public float orbitSpeed;
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
        float angleIncrease = 360 / w.amountOfProjectiles * Mathf.Deg2Rad * w.curvature;
        
        for (int i = 0; i < w.amountOfProjectiles; i++)
        {
            float c_Angle = angle - angleIncrease * w.amountOfProjectiles / 2 + angleIncrease / 2; 
            float xCurve = w.radius * (Mathf.Cos(c_Angle) - Mathf.Sin(c_Angle));
            float yCurve = w.radius * (Mathf.Sin(c_Angle) + Mathf.Cos(c_Angle));

            Gizmos.color = Gizmos.color == Color.green ? Color.blue : Color.green;

            float x = xCurve + w.separation * i - w.separation * w.amountOfProjectiles / 2 + w.separation / 2 + w.position.x;
            float y = yCurve + w.position.y;

            Vector2 from = new Vector2(x, yCurve);
            Vector2 to = new Vector2(x + w.globalDirection.normalized.x * arrowLength * w.radius / 2 + w.projectileSpread * (Mathf.Cos(c_Angle) - Mathf.Sin(c_Angle)),
                                     y + w.globalDirection.normalized.y * arrowLength * w.radius / 2 + w.projectileSpread * (Mathf.Sin(c_Angle) + Mathf.Cos(c_Angle)));

            Gizmos.DrawLine(from, to);
            //Arrowheads
           // Gizmos.DrawLine(to, new Vector2(to.x - 0.2f, to.y + 0.5f));
            //Gizmos.DrawLine(to, new Vector2(to.x + 0.2f, to.y + 0.5f));
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
            WaveData w = waveData[waveIndex];
            attackFrequency_Timer = 0;
            float displacement = 0;
            float angle = (180 + 90 / 2) * Mathf.Deg2Rad; //Sets it so the first point goes forward
            float angleIncrease = 360 / w.amountOfProjectiles * Mathf.Deg2Rad * w.curvature;

            for (int i = 0; i < w.amountOfProjectiles; i++)
            {
                float c_Angle = angle - angleIncrease * w.amountOfProjectiles / 2 + angleIncrease / 2;
                float xCurve = w.radius * (Mathf.Cos(c_Angle) - Mathf.Sin(c_Angle));
                float yCurve = w.radius * (Mathf.Sin(c_Angle) + Mathf.Cos(c_Angle));

                float x = xCurve + w.separation * i - w.separation * w.amountOfProjectiles / 2 + w.separation / 2 + w.position.x;
                float y = yCurve + w.position.y;

                displacement++; angle += angleIncrease;

                Vector2 spread = new Vector2(w.projectileSpread * (Mathf.Cos(c_Angle) - Mathf.Sin(c_Angle)),
                                             w.projectileSpread * (Mathf.Sin(c_Angle) + Mathf.Cos(c_Angle)));

                if(w.orbitSpeed <= 0)
                {
                    Attack(w.globalDirection + spread, new Vector2(x, y));
                }
                else
                {
                    Attack(w.globalDirection + spread, new Vector2(x, y), w.position, w.orbitSpeed);
                }
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

    public void Attack(Vector2 direction, Vector2 displacement, Vector2 origin, float orbitSpeed)
    {
        //For if the projectiles should orbit around the position of the spawner
        attack.Attack(direction, (Vector2)transform.position + displacement, transform.position, orbitSpeed, GetComponent<Collider>());
    }
}
