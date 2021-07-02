using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAsteroids : MonoBehaviour
{
    ObjectPooler objectPooler;

    public float innerRadius = 500f; //inner radius of ring that asteroids will be spawned
    public float outerRadius = 1200f; //outer radius of ring that asteroids will be spawned
   
    private void Start()
    {
        objectPooler = ObjectPooler.Instance;
    }

    private void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            print("spawning");
            objectPooler.SpawnFromPool("asteroid", RandomPos(), RandomRot());
        }
    }

    public Vector3 RandomPos()
    {

        float distx = Random.Range(innerRadius, outerRadius);
        float disty = Random.Range(innerRadius, outerRadius);
        float distz = Random.Range(innerRadius, outerRadius);
        return new Vector3(distx, disty, distz);
    }
    public Quaternion RandomRot()
    {
        return Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
    }

}
