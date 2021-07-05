using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StationScript : MDestroyable
{
    public Slider s_health;
    public Text t_health;
    public GameObject ExplosionGO;
    public GameObject Camera;
    public override void Explode()
    {
        Camera.GetComponent<CameraFollow>().enabled = false;
        Camera.GetComponent<CustomCrosshair>().enabled = false;
        Camera.transform.LookAt(transform);
        transform.DetachChildren();
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<MeshCollider>() != null)
            {
                child.gameObject.AddComponent<Rigidbody>().useGravity = false;
            }
            else
            {
                Destroy(child.gameObject);
            }
        }
        Time.timeScale = 0.25f;
        var explode = Instantiate(ExplosionGO);
        explode.transform.position = transform.position;
        explode.transform.localScale *= 25;
        Collider[] toBlow = Physics.OverlapSphere(transform.position, 500);
        foreach (Collider c in toBlow)
        {
            Rigidbody r = c.GetComponent<Rigidbody>();
            if (r != null)
            {
                r.AddExplosionForce(50f, transform.position, 20);
            }
        }
        Destroy(explode, 2.5f);
    }

    public override void TakeDamage(float dmg)
    {
        print("Took "+dmg +" damage to station");
        base.TakeDamage(dmg);
        s_health.value = health;
        t_health.text = health.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        health = 1500;
        Camera = GameObject.Find("Main Camera");
    }

}
