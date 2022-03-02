using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityBlink : AbilityBase {
    [SerializeField]
    float Cooldown;

    [SerializeField]
    int MaxCharges;

    float Timestamp;
    int Currentcharges;

    [SerializeField]
    float BlinkDistance;

    //(De)ActivateAbility is called by Player.cs
    void ActivateAbility()
    {
        if (Currentcharges > 0)
        {
            GameObject Particles = Instantiate(Resources.Load("Abilities/BlinkEffect")) as GameObject;
            Particles.GetComponent<ParticleSystem>().startColor = GameManager.Instance.CurrentEnergy;
            Particles.transform.position = this.transform.root.position;
            this.transform.root.position = new Vector3(this.transform.root.position.x, this.transform.root.position.y, this.transform.root.position.z + BlinkDistance);
            Currentcharges--;
            Debug.Log(Currentcharges);
            if (Time.time > Timestamp)
            {
                SetCooldown(Cooldown);
            }
        }
    }

    void DeactivateAbility()
    {

    }

    void SetCooldown(float seconds)
    {
        Timestamp = Time.time + seconds; 
    }


    // Use this for initialization
    void Start () {
        base.Start();
        if (transform.root.gameObject.GetComponent<Player>() != null)
        {
            player = transform.root.gameObject.GetComponent<Player>();
            Currentcharges = MaxCharges;
            if (player.AbilityUI != null)
            {
            player.AbilityUI.transform.GetChild(0).GetComponent<Image>().color = GameManager.Instance.CurrentEnergy;
                player.AbilityUI.GetComponent<Image>().sprite = icon;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Q) && !LockAbility)
        {
            ActivateAbility();
        }

        if (Time.time < Timestamp) {
            player.AbilityUI.transform.GetChild(0).GetComponent<Image>().fillAmount = (Timestamp - Time.time) / Cooldown; 
        }else if (Time.time > Timestamp && !LockAbility && Currentcharges < MaxCharges)
        {
            Currentcharges++;
            Debug.Log("Regen " + Currentcharges);

            if (Currentcharges < MaxCharges)
            {
                SetCooldown(Cooldown);
            }
        }
	}
}
