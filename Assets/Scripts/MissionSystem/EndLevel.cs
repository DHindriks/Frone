using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour {

    //Animator anim;
    bool TakeOff = false;
    float direction;
    Vector3 Speed;
    Rigidbody rb;
    Player player;
    void Start()
    {
        //anim = GetComponentInParent<Animator>();
        rb = GetComponentInParent<Rigidbody>();
    }
    // Update is called once per frame
    void Update () {
		if (TakeOff)
        {
            rb.AddForce(transform.TransformDirection(Speed) * 40, ForceMode.Force);

            rb.AddForce((20 * direction) * Time.deltaTime, 80 * Time.deltaTime, 0, ForceMode.Force);
            if (rb.velocity != Vector3.zero)
            transform.parent.LookAt(transform.parent.position + rb.velocity);
        }
    }

    void StartTakeOff()
    {
        TakeOff = true;
        rb.isKinematic = false;
        foreach (Transform child in player.transform)
        {
            child.gameObject.SetActive(false);
        }
        Invoke("EndLevelPhase2", 2.5f);
    }

    void EndLevelPhase2()
    {
        player.EndlevelPhase2();
        Speed = new Vector3(Random.Range(1, -2), 0, 1);
        GameManager.Instance.ObjectiveManager.SetUI(false);
        GameManager.Instance.AddReputation(PlayerPrefs.GetString("SupportingFaction", null), Mathf.FloorToInt(GameManager.Instance.ObjectiveManager.Score) / 10);
    }

    void DeactivateColliders(GameObject gameObject)
    {
        foreach (Collider coll in gameObject.GetComponents<Collider>())
        {
            coll.enabled = false;
        }
        foreach (Transform Child in gameObject.transform)
        {
            DeactivateColliders(Child.gameObject);
        }

    }



    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            transform.parent.parent.SetParent(null);
            transform.parent.SetParent(null);
            if (other.GetComponentInChildren<AbilityBase>())
            MissionEvents.ReachEnd.Invoke();
            GameManager.Instance.FloorManager.target = null;
            other.GetComponent<Player>().Invincible = true;
            other.GetComponent<Player>().Allowcontrols = false;
            other.GetComponent<Rigidbody>().isKinematic = true;
            player = other.GetComponent<Player>();
            other.transform.SetParent(this.transform);
            DeactivateColliders(transform.parent.gameObject);
            direction = Random.Range(-4, 4);
            Speed = new Vector3(0, 0, 1);
            //anim.SetBool("Open", false);
            Invoke("StartTakeOff", 4);
        }
    }
}
