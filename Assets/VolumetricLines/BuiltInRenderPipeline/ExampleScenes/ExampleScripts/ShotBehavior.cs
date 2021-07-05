using UnityEngine;
using System.Collections;
using System;

public class ShotBehavior : MonoBehaviour {
    public float dmg;
	// Use this for initialization
	void Start () {

		GetComponent<Rigidbody>().AddForce((transform.forward) * 15000f);
		StartCoroutine(Fired());
	}

    private IEnumerator Fired()
    {
        yield return new WaitForSeconds(0.35f);
        GetComponentInChildren<CapsuleCollider>().isTrigger = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        collision.collider.gameObject.TryGetComponent<MDestroyable>(out var md);
        if (md != null)
        {
            md.TakeDamage(dmg);
        }
        else if (collision.collider.transform.root.TryGetComponent<MDestroyable>(out var dest))
        {
            dest.TakeDamage(dmg);
        }
        Destroy(gameObject);
    }
}
