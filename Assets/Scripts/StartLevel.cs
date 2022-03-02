using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLevel : MonoBehaviour {

    [SerializeField]
    GameObject AnimatedShip;

    [SerializeField]
    GameObject Player;

    [SerializeField]
    ObjectiveManager scoremanager;
    bool done;

    private void Start()
    {
        Player.GetComponent<Rigidbody>().isKinematic = true;
        Invoke("StartGame", 2.5f);
        scoremanager.SetUI(false);
    }

    private void StartGame()
    {
        CancelInvoke("StartGame");
        Player.GetComponent<Rigidbody>().isKinematic = false;
        done = true;
        Invoke("StartChecking", 2.5f);
    }

    private void StartChecking()
    {
        Player.GetComponent<Player>().Checkspeed = true;
        scoremanager.gameObject.transform.position = Player.transform.position;
        scoremanager.SetUI(true);
        Invoke("SpawnFlyingShip", 5);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.W) && done == false)
        {
            StartGame();
        }
    }

    private void SpawnFlyingShip()
    {
        GameObject Ship = Instantiate(AnimatedShip); 
        Ship.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 20, Player.transform.position.z - 20);
    }

}
