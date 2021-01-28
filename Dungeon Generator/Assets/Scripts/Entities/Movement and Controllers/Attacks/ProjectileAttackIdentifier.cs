using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttackIdentifier : AttackIdentifier
{
    [System.Serializable]
    public struct WaveData
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
    int waveIndex;

    public int waveFrequency_Limit; //Frequency between the waves, not the Attack Frequency.
    int waveFrequencyTimer;

    public override void Initialize()
    {
        waveFrequencyTimer = 0;
        waveIndex = 0;
    }

    public override void OnFixedUpdate(Vector3 direction, Vector3 source, Collider collider)
    {
        UpdateCasting();
        waveFrequencyTimer++;
       // Debug.Log("Index: " + waveIndex + "Data count: " + waveData.Count);
        if (waveFrequencyTimer >= waveFrequency_Limit && waveIndex < waveData.Count)
        {
            WaveData w = waveData[waveIndex];
            waveFrequencyTimer = 0;
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

                if (w.orbitSpeed == 0)
                {
                    OnAttack(((Vector3)w.globalDirection + direction).normalized + (Vector3)spread, source + new Vector3(x, y, 0), collider);
                }
                else
                {
                    OnAttack(((Vector3)w.globalDirection + direction).normalized + (Vector3)spread, source + new Vector3(x, y, 0), w.position, w.orbitSpeed, collider);
                }
            }
            waveIndex++;
        }
    }

    [SerializeField]ProjectileController projectile;
    protected override void OnAttack(Vector3 direction, Vector3 source, Collider collider)
    {
        ProjectileController temp = Instantiate(projectile, source, Quaternion.identity);
        SetDirectionAndRotate(ref temp, direction);
        Debug.Log("Ignore between " + collider.gameObject.name + " and " + temp.gameObject.name);
        Physics.IgnoreCollision(collider, temp.GetComponent<SphereCollider>());
    }
    protected override void OnAttack(Vector3 direction, Vector3 source, Vector3 origin, float orbitSpeed, Collider collider)
    {
        ProjectileController temp = Instantiate(projectile, source, Quaternion.identity);
        SetDirectionAndRotate(ref temp, direction);
        temp.orbitPoint = origin; temp.orbitSpeed = orbitSpeed;
        Physics.IgnoreCollision(collider, temp.GetComponent<Collider>());
    }
    void SetDirectionAndRotate(ref ProjectileController projectile, Vector3 direction)
    {
        projectile.Dir = direction;
        float dot = Vector2.Dot(direction, Vector2.up);
        float angle = Mathf.Asin(direction.x) * Mathf.Rad2Deg;
        angle = dot > 0 ? -(angle + 180) : angle;
        projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
