using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItems : MonoBehaviour
{
    public struct DropData
    {
        public GameObject objectToDrop;
        float dropLikelihood;

        public DropData(GameObject object_in, float dropLikelihood_in)
        {
            objectToDrop = object_in;
            dropLikelihood = dropLikelihood_in;
        }
    }
    List<DropData> itemsToDrop = new List<DropData>();

    public void Initialize(Currency currency_in)
    {
        itemsToDrop.Add(new DropData(currency_in.smallCoins[0], 1));
        itemsToDrop.Add(new DropData(currency_in.smallCoins[1], 1));
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Drop(5, Vector3.zero);
        }
    }

    public void Drop(float impactStrength, Vector3 impactDirection)
    {
        //The direction can determine if the items fly away in another direction, perhaps because of a bomb or strong blow
        //The strength is also determined by how strong the attack was
        for(int i = 0; i < 10; i++)
        {
            GameObject temp = Instantiate(itemsToDrop[Random.Range(0, itemsToDrop.Count)].objectToDrop, transform.position, Quaternion.identity);
            Vector3 forceDirection = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f,1.0f), 0).normalized;
            //Debug.Log(forceDirection);
            temp.GetComponent<Rigidbody>().AddForce(forceDirection * (impactStrength * Random.Range(1.0f, 1.5f)), ForceMode.Impulse);
            VisualsRotator.Add(temp.GetComponentInChildren<MeshRenderer>());
        }
    }

}