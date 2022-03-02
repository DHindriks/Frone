using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropshipAttack : MonoBehaviour {
    Rigidbody rb;
    Camerascript cam;
    GameObject CamPosTarget;

    public float ActiveTime;
    float ActiveTimeLeft;
    bool SetupComplete;
    [HideInInspector]
    public Player player;

	// Use this for initialization
	void Start () {
        ActiveTimeLeft = ActiveTime;
        //Invoke("SetUp", 0.002f);
        SetUp();
    }

    void SetUp()
    {
        player = GameManager.Instance.player.GetComponent<Player>();
        player.FreezePlayer(true);
        player.fuelbar.GetComponent<Image>().color = GetComponentInChildren<DroneSettings>().EnergyColor;
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camerascript>();
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.TransformDirection(Vector3.forward * 5), ForceMode.VelocityChange);
        CamPosTarget = new GameObject();
        CamPosTarget.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 20);
        CamPosTarget.transform.SetParent(transform);
        cam.SetcamPos(CamPosTarget, transform.parent.gameObject, 0, 0, 10, 2);
        DialogueManager.Instance.AddDialogue("Starting attack run!", Characters.Zero);
        DialogueManager.Instance.AddDialogue("Aim for me, pilot", Characters.Zero);
        SetupComplete = true;
    }

    // Update is called once per frame
    void Update () {
        if (SetupComplete)
        {
            ActiveTimeLeft -= Time.deltaTime;
            if (ActiveTimeLeft <= 0)
            {
                EndAttack();
                Debug.Log("ATTACK ENDED");
            }else
            {
                player.fuelbar.transform.localScale = new Vector3(0.75f, ActiveTimeLeft / ActiveTime, 0.75f);
            }
        }
    }

    void EndAttack()
    {
        Destroy(GetComponentInChildren<ShipWeaponBase>());
        cam.SetcamPos();
        player.fuelbar.GetComponent<Image>().color = player.GetComponentInChildren<DroneSettings>().EnergyColor;
        player.FreezePlayer(false);
    }
}
