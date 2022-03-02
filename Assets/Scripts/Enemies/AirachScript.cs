using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirachScript : HealthObj {

    enum states
    {
        Patrol,
        Follow,
        Attack,
        Dead
    }

    states State;
    GameObject Target;
    Vector3 TargetPos;
    Vector3 LookPos;

    float RdmPatrolSpeed;

    [SerializeField]
    GameObject LookAt;
    GameObject laser;


    void Awake()
    {
        LookAt = this.transform.root.gameObject;
        RdmPatrolSpeed = Random.Range(1, 8);
        laser = transform.GetChild(0).gameObject;
        State = states.Patrol;
    }

    // Use this for initialization
    void OnTriggerEnter(Collider col) {
        if (col.tag == "Player")
        {
            transform.parent = null;
            Target = col.gameObject;
            LookAt = col.gameObject;
            Activate();
        }
	}

    private void OnCollisionStay(Collision collision)
    {
        if (State != states.Dead)
        {
            Die();
            GetComponent<ParticleSystem>().Play();
        }
    }

    void Die()
    {
        State = states.Dead;
        Destroy(laser);
        this.GetComponent<Rigidbody>().isKinematic = true;
        CancelInvoke();
        Destroy(this.gameObject, 10);
    }

    void Activate()
    {
        if (State != states.Dead)
        {
            //do sound/animation here
            laser.GetComponentInChildren<Collider>().enabled = false;
            Invoke("FollowPlayer", 1f);
        }
    }

    void FollowPlayer()
    {
        if (State != states.Dead)
        {
        State = states.Follow;
        Destroy(laser);
        Invoke("Attack", 6f);
        }
    }

    void Attack()
    {
        if (State != states.Dead)
        {
            State = states.Attack;
            this.GetComponent<Rigidbody>().velocity = Target.GetComponent<Rigidbody>().velocity;
            Invoke("Die", 5);
        }
    }

	// Update is called once per frame
	void Update () {
		
        if(State == states.Patrol)
        {
            if (LookAt != null)
            {
                LookPos = new Vector3(Mathf.PingPong(Time.time * RdmPatrolSpeed, 16) - 8, LookAt.transform.position.y, LookAt.transform.position.z);
            }

        } else if (State == states.Follow)
        {
            TargetPos.Set(Target.transform.position.x, Target.transform.position.y + 6, Target.transform.position.z + 6);
            LookPos = new Vector3(LookAt.transform.position.x, LookAt.transform.position.y, LookAt.transform.position.z);
            //this.GetComponent<Rigidbody>().MovePosition(TargetPos);
            //this.GetComponent<Rigidbody>().velocity = Target.GetComponent<Rigidbody>().velocity;
            this.GetComponent<Rigidbody>().AddForce((TargetPos - transform.position) * 2);
            //this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(Target.transform.position.x, Target.transform.position.y + 6, Target.transform.position.z + 6), 0.1f);
        }
        else if (State == states.Attack)
        {
            this.GetComponent<Rigidbody>().AddForce(0, (-Target.GetComponent<Player>().speed / 2) * Time.deltaTime, 0, ForceMode.Force);
        }

        if (LookAt != null && laser != null)
        {
            laser.transform.LookAt(LookPos);
        }
	}
}
