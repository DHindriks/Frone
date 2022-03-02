using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObstacles : MonoBehaviour {

    [SerializeField]
    GameObject Effect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Obstacle" || other.gameObject.tag == "Danger")
        {
            GameObject particles = Instantiate(Effect);
            particles.transform.position = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            other.enabled = false;
            Time.timeScale = 1f;
            Invoke("ResetTimeScale", 5);
        }
    }

    void ResetTimeScale()
    {
        Time.timeScale = 1;
    }

    private void OnTriggerEXit(Collider other)
    {
        if (other.gameObject.tag == "Obstacle" || other.gameObject.tag == "Danger")
        {
            other.enabled = true;
        }
    }


}
