using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObstacles : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {


        if (other.gameObject.tag == "Obstacle" || other.gameObject.tag == "Danger")
        {
            Destroy(other.gameObject);
        }
    }
}