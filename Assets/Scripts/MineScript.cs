using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineScript : MDestroyable
{
    public GameObject explosion;


    public float explosionRadius = 120f;
    public float explosionForce = 50000f;


    public override void Explode()
    {
        var expo = Instantiate(explosion, transform.position, transform.rotation);
        expo.transform.localScale *= 7;
        Collider[] toBlow = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider c in toBlow)
        {
            if (c.gameObject.TryGetComponent<MDestroyable>(out var fra) && c.gameObject != this.gameObject)
            {
                fra.Explode();
            }
            Rigidbody r = c.GetComponent<Rigidbody>();
            if (r != null)
            {
                r.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
        gameObject.SetActive(false);
        Destroy(expo, 2.5f);

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Rocket" && collision.collider.gameObject.transform.root != null)
        {
            print("hit!");
            if (collision.collider.gameObject.transform.root.tag == "Player")
            {
                collision.collider.gameObject.transform.root.GetComponent<SpaceShipControl>().CashUpdate(500);
                print("cash");
            }
        }
        Explode();
    }
}
