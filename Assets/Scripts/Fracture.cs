using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class MDestroyable:MonoBehaviour
{
    [SerializeField]
    public float health;
    public abstract void Explode();

    public virtual void TakeDamage(float dmg)
    {
        health -= dmg;
        print("Remaining health: "+health);
        if(health <= 0)
        {
            Explode();
        }
    }
}
public class Fracture : MDestroyable
{
    public GameObject fractured;
    Rigidbody rb;
    private Canvas canvas;
    private GameObject arrow;
    private GameObject Player;

    private void Awake()
    {
        Player = GameObject.Find("Spaceship Container");
        canvas = GetComponentInChildren<Canvas>();
        health = 100;
    }

    private void Update()
    {
        canvas.transform.LookAt(Player.transform.position);
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
        var root = collision.collider.gameObject.transform.root;
            if (root.tag == "Station")
            {
                root.GetComponent<MDestroyable>().TakeDamage(200);
                this.TakeDamage(200);
            }
    }



}
