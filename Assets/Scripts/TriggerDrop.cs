using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDrop : MonoBehaviour {

    public event EventHandler OnDeliver;
    public event EventHandler OnFail;


    bool PackageArrived = false;
    [SerializeField]
    Transform Deliverpos;


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            MissionController.Instance.CurrentMissionManager.GetComponent<DeliveryManager>().Rope.GetComponentInChildren<DeliveryPackage>().SetDrop(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "DeliverPackage")
        {
            Debug.Log(other.GetComponent<Rigidbody>().velocity.magnitude);
            if (other.GetComponent<Rigidbody>().velocity.magnitude <= 0.5f && !PackageArrived)
            {
                //activate event: objective complete

                Debug.LogError("Package arrived at " + Vector3.Distance(other.transform.position, Deliverpos.position) + " from the target");
                if (Vector3.Distance(other.transform.position, Deliverpos.position) < 20)
                {
                    PackageArrived = true;
                    if (OnDeliver != null)
                    OnDeliver.Invoke(this, EventArgs.Empty);
                    DialogueManager.Instance.AddDialogue("Package delivered within range.", Characters.Zero);
                    DialogueManager.Instance.AddDialogue("Good work. Get to the extraction point.", Characters.Zero);
                }
            }
        }
    }

}
