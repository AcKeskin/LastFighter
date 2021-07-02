using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollisionHandler : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Rocket" && other.gameObject.tag != "Laser" && other.gameObject.tag != "Interaction")
        {
            transform.root.GetComponent<SpaceShipControl>().OnChildCollided();
        }
    }

}
