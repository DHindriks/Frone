using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDestabilize : AbilityBase {
    float MinCooldown = 5f;
    float VarCooldown = 5f;
    float CooldownTime;
    float Timestamp;
    float BeginTime;
    bool IsActive;
    [SerializeField]
    GameObject ShieldPrefab;


    GameObject Shield;


    // Use this for initialization
    void Start () {
        base.Start();
        if (transform.root.gameObject.GetComponent<Player>() != null)
        {
            player = transform.root.gameObject.GetComponent<Player>();
            if (player.AbilityUI != null)
            {
                player.AbilityUI.transform.GetChild(0).GetComponent<Image>().color = GameManager.Instance.CurrentEnergy;
                player.AbilityUI.GetComponent<Image>().sprite = icon;
            }
        }
	}

    void ToggleAbility()
    {
        if (Time.time > Timestamp)
        {
            Setability(!IsActive);
        }
    }

    bool Setability(bool active)
    {
        if (active == true)
        {
            if (Shield != null)
            {
                Shield.GetComponent<ParticleSystem>().Play();
                Shield.GetComponent<BoxCollider>().enabled = true;
            } else
            {
                Shield = Instantiate(ShieldPrefab, transform.root);
            }
            IsActive = true;
            BeginTime = Time.time;
        }else
        {
            IsActive = false;
            Shield.GetComponent<ParticleSystem>().Stop();
            Shield.GetComponent<BoxCollider>().enabled = false;
            CooldownTime = MinCooldown + (Time.time - BeginTime);
            SetCooldown(CooldownTime);
        }

        return IsActive;
    }

    void SetCooldown(float seconds)
    {
        Timestamp = Time.time + seconds; 
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Q) && !LockAbility)
        {
            ToggleAbility();
        }

        if (IsActive && player.gameover || IsActive && LockAbility)
        {
            ToggleAbility();
        }


        if (Time.time < Timestamp  && IsActive == false)
        {
            player.AbilityUI.transform.GetChild(0).GetComponent<Image>().fillAmount = (Timestamp - Time.time) / (MinCooldown + VarCooldown);
        } else if (IsActive == true)
        {
            player.AbilityUI.transform.GetChild(0).GetComponent<Image>().fillAmount = (Time.time - BeginTime) / VarCooldown;
            if (BeginTime + VarCooldown < Time.time)
            {
                ToggleAbility();
            }
        }


    }
}