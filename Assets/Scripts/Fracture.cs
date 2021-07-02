using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class MDestroyable:MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100;
    [SerializeField]
    private float health;

    private void Awake()
    {
        health = maxHealth;
    }
    public abstract void Explode();

    public void TakeDamage(float dmg)
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

    public override void Explode()
    {
        var fractd = Instantiate(fractured, transform.position, transform.rotation);
            fractd.transform.localScale = gameObject.transform.localScale; //Spawn in the broken version
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
