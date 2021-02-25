using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class ProjectileController : EntityMovementModel
{
    [System.Serializable]
    public struct targetData
    {
        public GameObject target;
        public int pushIndex;

        public targetData(GameObject target_in, int pushIndex_in)
        {
            target = target_in;
            pushIndex = pushIndex_in;
        }
    };
    public enum ProjectileAccelerationMode
    {
        NONE = 0,
        ACCELERATE = 1,
        DEACCELERATE = 2
    }
    public enum HomingMode
    {
        NONE = 0,
        HOMING = 1,
        REPULSED = 2, //goes in the opposite direction of the target, may hit allies instead because of it, or bounce off walls in unexpected manners
        HOMING_REPULSED = 3 //goes after player and tries to stay at a distance from them
    }
    public float blastRadius;
    public float explosionPower;
    public GameObject currentTarget;
    [SerializeField]ProjectileAccelerationMode accelerationMode;
    [SerializeField]HomingMode homingMode;

    public List<targetData> targets = new List<targetData>();
    public float gravitySpeed;

    public bool placedProjectile;

    public bool collideWithCaster;
    MeshRenderer renderer;

    [SerializeField]List<GameObject> visuals;

    public int lifeLength;
    int lifeTimer = 0;
    void Start()
    {
        Fric = 0.0f;
        Acc = new Vector2(1,1);
        VisualsRotator.renderers.AddRange(visuals);
        GetComponent<SphereCollider>().radius = blastRadius;
        GetComponent<SphereCollider>().isTrigger = true;
        renderer = GetComponentInChildren<MeshRenderer>();
        if(GetComponentInChildren<Light>())
        {
            GetComponentInChildren<Light>().color =renderer.sharedMaterial.color;
        }
        if(placedProjectile)
        {
            VisualsRotator.Add(renderer);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Dir);
    }
    new private void FixedUpdate() 
    {
        currentSpeed = speed;
        lifeTimer++;
        renderer.material.SetFloat("_IsExploding", (float)lifeTimer/(float)lifeLength);
        CheckAccelerationMode();
        CheckHomingMode();
        Move();
        if(lifeTimer >= lifeLength)
        {
            Destroy(this.gameObject);
        }
    }
    protected virtual void CheckAccelerationMode()
    {
        switch(accelerationMode)
        {
            case ProjectileAccelerationMode.ACCELERATE:
                //Speed up projectile overtime
                break;
            case ProjectileAccelerationMode.DEACCELERATE:
                //Slow down projectile overtime
                break;
            default:
                break;
        }
    }
    protected virtual void CheckHomingMode()
    {
        switch(homingMode)
        {
            case HomingMode.HOMING:
                break;
            case HomingMode.REPULSED:
                break;
            case HomingMode.HOMING_REPULSED:
                break;
            default: 
                break;
        }
    }
    private void OnAttackStay(GameObject vic)
    {
        bool isNew = true;
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].target != vic)
            {
                isNew = true;
            }
            else
            {
                isNew = false;
                break;
            }
        }
        if (isNew)
        {
            Vector2 vectorToTarget = (Vector2)(transform.position - vic.transform.position);
            float distanceModifier = vectorToTarget.magnitude <= blastRadius ? (blastRadius - vectorToTarget.magnitude) / blastRadius : 0;
            Vector2 pushV2 = vectorToTarget.normalized * gravitySpeed * distanceModifier;
            targets.Add(new targetData(vic, vic.GetComponent<EntityMovementModel>().AddPushVector(pushV2)));
            //Debug.Log("Adding: " + vic + " index: " + targets[targets.Count - 1].pushIndex + " at: " + vic.transform.position);
        }
        else
        {
            if(gravitySpeed != 0){GravityEffect(vic);}
        }
    }
    void GravityEffect(GameObject vic)
    {
        if (targets.Count > 0)
        {
            targetData targetVic = targets[0]; //If the list isnt empty, take the first element
            foreach (targetData t in targets)
            {
                if (t.target == vic) //Check if you already have the new element
                {
                    targetVic = t; //If you do, then your target is it
                    break;
                }
            }
            if (targetVic.target != null)
            {
                Vector2 vectorToTarget = (Vector2)(transform.position - vic.transform.position);
                float distanceModifier = vectorToTarget.magnitude <= blastRadius ? (blastRadius - vectorToTarget.magnitude) / blastRadius : 0;
                Vector2 value = vectorToTarget.normalized * gravitySpeed * distanceModifier;
                targetVic.target.GetComponent<EntityMovementModel>().push[targetVic.pushIndex] = value;

            }
        }
    }
    void Explode()
    {
        foreach(targetData t in targets)
        {
            if(t.target != null)
            {
                Vector2 vectorToTarget = (Vector2)(transform.position - t.target.transform.position);
                float distanceModifier = vectorToTarget.magnitude <= blastRadius ? (blastRadius - vectorToTarget.magnitude) / blastRadius : 0;
                Vector2 value = vectorToTarget.normalized * explosionPower * distanceModifier;
                t.target.GetComponent<EntityMovementModel>().push[t.pushIndex] = -value;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(gameObject.tag != other.gameObject.tag && other.gameObject.GetComponent<EntityMovementModel>())
        {
            OnAttackStay(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        foreach (targetData t in targets)
        {
            if (t.target == other.gameObject)
            {
                other.gameObject.GetComponent<EntityMovementModel>().RemovePushVector(t.pushIndex);
                targets.Remove(t);
                break;
            }
        }
    }
    public virtual void OnDestroy()
    {
        if(blastRadius > 0 && explosionPower > 0){Explode();}
        Destroy(this.gameObject);
    }
}
