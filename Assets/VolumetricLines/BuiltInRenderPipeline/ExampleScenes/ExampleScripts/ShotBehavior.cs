using UnityEngine;
using System.Collections;
using System;

public class ShotBehavior : MonoBehaviour {

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
        Destroy(gameObject);
    }
}
