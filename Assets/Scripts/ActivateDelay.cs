using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateDelay : MonoBehaviour {

    enum ActivateModes
    {
        TimeDelay,
        RangeCheck
    }
    [SerializeField]
    ActivateModes Mode;

    [SerializeField]
    int delay;

    [SerializeField]
    List<GameObject> Activatables;


	// Use this for initialization
	void Awake () {
        if (Mode == ActivateModes.TimeDelay)
        {
            Invoke("Activate", delay);
        }
	}
	
	void Activate ()
    {
        foreach(GameObject gameObject in Activatables)
        {
            gameObject.SetActive(true);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Activate();
        }
    }
}
