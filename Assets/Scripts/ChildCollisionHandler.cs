using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollisionHandler : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        print(other.name + " hit ship");
        if (other.transform.root.tag.Contains("Station"))
        {
            transform.root.GetComponent<MDestroyable>().TakeDamage(15);
        }
        else if (other.transform.root.tag.Contains("Rock"))
        {
            transform.root.GetComponent<MDestroyable>().TakeDamage(25);
            other.transform.GetComponent<MDestroyable>().TakeDamage(25);
        }
    }
    /*private void OnCollisionEnter(Collision collision)
    {
        print(collision.collider.name + " hit ship");
        if (collision.collider.transform.root.tag.Contains("Station"))
        {
            transform.root.GetComponent<MDestroyable>().TakeDamage(15);
        }
        else if (collision.collider.transform.root.tag.Contains("Rock"))
        {
            transform.root.GetComponent<SpaceShipControl>().TakeDamage(25);
            collision.collider.transform.GetComponent< MDestroyable>().TakeDamage(25);
        }
    }*/

}
