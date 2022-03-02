using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipFlyBy : MonoBehaviour {

    [SerializeField]
    float speed;

    [SerializeField]
    int DestroyDelay = 20;

    Rigidbody rb;

	// Use this for initialization
	void Awake () {
        rb = GetComponent<Rigidbody>();
        gameObject.transform.SetParent(null);
        Destroy(gameObject, DestroyDelay);
	}
	
	// Update is called once per frame
	void Update () {
        rb.AddForce(transform.TransformDirection(Vector3.forward) * speed, ForceMode.Force);
    }
}
