using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class MDestroyable:MonoBehaviour
{
    [SerializeField]
    public float health;
    public abstract void Explode();

    public virtual void TakeDamage(float dmg)
    {
        health -= dmg;
        print("Took " + dmg + " dmg");
        if(health <= 0)
        {
            Explode();
        }
    }
}
public class Fracture : MDestroyable
{
    public GameObject fractured;

    private void Awake()
    {
        health = 100;
    }

    public override void Explode()
    {
        var fractd = Instantiate(fractured, transform.position, transform.rotation);
            fractd.transform.localScale = gameObject.transform.localScale; //Spawn in the broken version
        var rbs = fractd.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rbs)
        {
            rb.AddExplosionForce(50f,fractd.transform.position,5f);
        }
        Destroy(fractd, 2f);
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Rocket" && collision.collider.gameObject.transform.root != null)
        {
            if (collision.collider.gameObject.transform.root.tag == "Player")
            {
                collision.collider.gameObject.transform.root.GetComponent<SpaceShipControl>().CashUpdate(250);
                this.TakeDamage(collision.collider.gameObject.transform.root.GetComponent<SpaceShipControl>().bulletDMGs[1]);
            }
        }
        else if(collision.collider.gameObject.tag == "Laser" && collision.collider.gameObject.transform.root != null)
        {
            if (collision.collider.gameObject.transform.root.tag == "Player")
            {
                collision.collider.gameObject.transform.root.GetComponent<SpaceShipControl>().CashUpdate(250);
                this.TakeDamage(collision.collider.gameObject.transform.root.GetComponent<SpaceShipControl>().bulletDMGs[2]);
            }
        }
    }



}
