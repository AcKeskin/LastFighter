using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketMovement : MonoBehaviour
{
    Rigidbody rb;
    ParticleSystem ps;
    public float lifetime = 7f;//Lifetime of rocket in terms of seconds
    public float explosionRadius = 20f;//The radius of explosion
    private float explosionForce = 5000f;// The strength of explosion
    public GameObject explosion ;
    public float dmg;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ps = GetComponentInChildren<ParticleSystem>();
        ps.Stop();
        StartCoroutine(Fired());
    }

    public IEnumerator Move()
    {
        GetComponentInChildren<MeshCollider>().isTrigger = true;
        yield return new WaitForSeconds(0.75f);
        rb.velocity = Vector3.zero;
        if(ps!=null)
            ps = GetComponentInChildren<ParticleSystem>();
        ps.Play();
        GetComponentInChildren<MeshCollider>().isTrigger = false;
        rb.AddForce((transform.forward) * 15000f);
    }
    public IEnumerator Fired()
    {
        StartCoroutine(Move());
        yield return new WaitForSeconds(lifetime);
        Destroy(this.gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.TryGetComponent<MDestroyable>(out var md);
        if(md != null)
        {
            md.TakeDamage(dmg);
        }
        else if (collision.collider.transform.root.TryGetComponent<MDestroyable>(out var dest))
        {
            dest.TakeDamage(dmg);
        }

            Explode();
            Destroy(this.gameObject);

    }

    public void Explode()
    {
        var expo = Instantiate(explosion, transform.position, transform.rotation);
        Collider[] toBlow = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider c in toBlow)
        {
            Rigidbody r = c.GetComponent<Rigidbody>();
            if (r != null)
            {
                r.AddExplosionForce(explosionForce, c.transform.position, explosionRadius);
            }
        }
        Destroy(expo, 2.5f);

    }
}
