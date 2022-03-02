using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackArea : MonoBehaviour {
    [SerializeField]
    GameObject Send;
    [SerializeField]
    GameObject Recieve;
    [SerializeField]
    GameObject Connectedto;
    [SerializeField]
    bool AutoReset = true;

    HackManager manager;
    Camerascript cam;
    bool Activated = false;
    GameObject connectedpartobj;
    GameObject recievingpartobj;

    void Start()
    {
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camerascript>();
        manager = GameObject.FindWithTag("UIManager").GetComponent<HackManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            connectedpartobj = Instantiate(Send, other.transform);
            recievingpartobj = Instantiate(Recieve);
            //cam.SetcamPos(null, Connectedto, 0.75f, 0, 10, 1.5f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            connectedpartobj.transform.LookAt(Connectedto.transform);
            recievingpartobj.transform.position = Connectedto.transform.position;
            recievingpartobj.transform.LookAt(other.transform);
            if (Input.GetKeyDown(KeyCode.E) && Activated == false)
            {
                Activated = true;
                Destroy(GetComponent<Collider>());
                RemoveParticles();
                //cam.SetcamPos();
                manager.StartSequence(5, Connectedto, Connectedto, AutoReset);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            RemoveParticles();
            //cam.SetcamPos();
        }
    }

    void RemoveParticles()
    {
        if (connectedpartobj.GetComponent<ParticleSystem>())
        {
            connectedpartobj.GetComponent<ParticleSystem>().loop = false;
        }
        else
        {
            Destroy(connectedpartobj);
        }

        if (recievingpartobj.GetComponent<ParticleSystem>())
        {
            recievingpartobj.GetComponent<ParticleSystem>().loop = false;
        }
        else
        {
            Destroy(recievingpartobj.GetComponent<Collider>());
        }
    }
}
