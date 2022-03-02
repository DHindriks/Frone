using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFighter : HealthObj
{
    Animator animator;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(null);
        animator = GetComponentInChildren<Animator>();
        player = GameManager.Instance.player;
        animator.SetInteger("AtkID", Random.Range(0, 2));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y, player.transform.position.z), 10 * Time.deltaTime);
    }
}
