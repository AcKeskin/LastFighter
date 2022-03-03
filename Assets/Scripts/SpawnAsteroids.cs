using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnAsteroids : MonoBehaviour
{
    ObjectPooler objectPooler;
    EnemyIndicatorManager iManager;

    public float innerRadius = 500f; //inner radius of ring that asteroids will be spawned
    public float outerRadius = 1200f; //outer radius of ring that asteroids will be spawned
   
    private void Start()
    {
        objectPooler = ObjectPooler.Instance;
        iManager = GameObject.FindObjectOfType<EnemyIndicatorManager>();
    }

    private void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            print("spawning");
            var item = objectPooler.SpawnFromPool("asteroid", RandomPos(), RandomRot());
            iManager.BindIndicator(item);
        }
    }
    
    public Vector3 RandomPos()
    {
        return Random.onUnitSphere* Random.Range(innerRadius, outerRadius);
    }
    public Quaternion RandomRot()
    {
        return Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
    }

}
