using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGravityZone : MonoBehaviour {

    [SerializeField]
    bool Resetter;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Vector3 NewGravity = transform.up * 9.8f;
            other.GetComponent<Player>().ChangeGravity(NewGravity.x, NewGravity.y, NewGravity.z);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && Resetter)
        {
            other.GetComponent<Player>().ChangeGravity();
        }
    }
}
