using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryPackage : MonoBehaviour {

    Rigidbody rb;
    float flyheight = 0.5f;
    bool CanDrop = false;
    [SerializeField]
    ParticleSystem UnlockedParticles;
    [SerializeField]
    ParticleSystem DisConnectParticles;
    [SerializeField]
    List<GameObject> CableObjects;

    // Use this for initialization
    void Start () {
        rb = this.GetComponent<Rigidbody>();
        foreach (GameObject gameObject in CableObjects)
        {
            SetColors(gameObject);
        }
	}

    void SetColors(GameObject gameObject)
    {
        foreach (Transform child in gameObject.transform.root)
        {
            if (child.GetComponent<Renderer>() != null)
            {
                child.GetComponent<Renderer>().material.SetColor("_Color", GameManager.Instance.CurrentEnergy);
                child.GetComponent<Renderer>().material.SetColor("_EmissionColor", GameManager.Instance.CurrentEnergy);
            }
            if (child.GetComponentInChildren<ParticleSystem>() != null)
            {
                SetParticleColors(child.gameObject);
            }
        }
    }

    void SetParticleColors(GameObject gameObject)
    {
        foreach (ParticleSystemRenderer renderer in gameObject.GetComponentsInChildren<ParticleSystemRenderer>())
        {
            foreach(Material mat in renderer.materials)
            {
                mat.SetColor("_Color", GameManager.Instance.CurrentEnergy);
                mat.SetColor("_EmissionColor", GameManager.Instance.CurrentEnergy);
            }
        }
    }

    public void SetDrop (bool enabled)
    {
        CanDrop = enabled;
        if(enabled)
        {
            UnlockedParticles.Play();
        }else
        {
            UnlockedParticles.Stop();
        }
    }

    void Drop()
    {
        DisConnectParticles.Play();
        SetDrop(false);

        foreach(Transform Child in transform.root)
        {
            if (Child.tag != "DeliverPackage")
            {
                if (Child.GetComponent<Rigidbody>())
                Child.GetComponent<Rigidbody>().useGravity = false;
                if (Child.GetComponent<ConfigurableJoint>())
                Destroy(Child.GetComponent<ConfigurableJoint>());
                GameManager.Instance.ShrinkDespawn(Child.gameObject);
            }
        }
    }

	// Update is called once per frame
	void Update () {
        //gravity
        rb.AddForce((Physics.gravity * rb.mass) * (5.5f) * Time.deltaTime);
        if (rb.velocity.magnitude > 1f)
        {
            transform.LookAt(transform.position + rb.velocity);

            RaycastHit hit;
            Ray Distancetoground = new Ray(transform.position, -Vector3.up);
            if (Physics.Raycast(Distancetoground, out hit))
            {
                if (hit.distance < flyheight)
                {
                    rb.AddForce(0, (rb.mass * (flyheight - hit.distance) / flyheight) * (5.5f) * Time.deltaTime, 0, ForceMode.Acceleration);
                    rb.AddForce(0, 5, 0);
                }
            }
        }

        if (CanDrop && Input.GetKeyDown(KeyCode.E))
        {
            Drop();
        }

    }
}
