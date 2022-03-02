using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAnimation : MonoBehaviour {

    Player player;
    bool Despawning = false;
    float direction;

	// Use this for initialization
	void Start () {
        player = GameManager.Instance.player.GetComponent<Player>();
        Invoke("Despawn", 5);
        direction = Random.Range(-4, 4);
    }
	
    void Despawn()
    {
        Despawning = true;
        Destroy(this.gameObject, 5);
    }

	// Update is called once per frame
	void Update () {
        this.transform.LookAt(this.transform.position + this.GetComponent<Rigidbody>().velocity);

        if (Despawning == false)
        {
            this.GetComponent<Rigidbody>().AddForce(0, 0, (player.speed * 2) * Time.deltaTime, ForceMode.Force);
        } else
        {
            this.GetComponent<Rigidbody>().AddForce((player.speed * direction) * Time.deltaTime, (player.speed * 4) * Time.deltaTime, 0, ForceMode.Force);
        }
    }
}
