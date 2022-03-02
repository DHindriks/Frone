using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiscordPresence;

public class Onstartup : MonoBehaviour {

    [SerializeField]
    GameObject TimeManager;

    [SerializeField]
    List<GameObject> ObjsToSpawn;

	// Use this for initialization
	void Awake () {

        if (GameObject.FindWithTag("GameManager") == null)
        {
            foreach (GameObject gameObject in ObjsToSpawn)
            {
                Instantiate(gameObject);
            }
        }

        if (!PlayerPrefs.HasKey("Model"))
        {
            FirstStartup();
            Debug.Log("first startup detected");
        }
        PresenceManager.UpdatePresence(detail: "Hangar");
	}

    void FirstStartup()
    {
        PlayerPrefs.SetString("Model", "Default");
        GameManager.Instance.UnlockDrone("Default");
        GameManager.Instance.UnlockDrone("Prototype");
    }
}
