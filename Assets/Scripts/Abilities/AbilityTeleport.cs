using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityTeleport : AbilityBase {
    [SerializeField]
    float Cooldown;

    float Timestamp;
    int Currentcharges;

    bool IsActive;
    GameObject TPIndicator;
    [SerializeField]
    float MaxDistance;
    float CurrentDist;

    [SerializeField]
    GameObject CloneEffectsPrefab;

    [SerializeField]
    GameObject PlayerEffectsPrefab;

    //(De)ActivateAbility is called by Player.cs
    void ActivateAbility()
    {
        if (Time.time > Timestamp)
        {
            GameObject Particles = Instantiate(Resources.Load("Abilities/BlinkEffect")) as GameObject;
            Particles.GetComponent<ParticleSystem>().startColor = GameManager.Instance.CurrentEnergy;
            Particles.transform.position = this.transform.root.position;
            //this.transform.root.position = new Vector3(this.transform.root.position.x, this.transform.root.position.y, this.transform.root.position.z + BlinkDistance);
            Currentcharges--;
            Debug.Log(Currentcharges);
            SetCooldown(Cooldown);
        }
    }

    void SetCooldown(float seconds)
    {
        Timestamp = Time.time + seconds; 
    }


    // Use this for initialization
    void Start () {
        base.Start();
        IsActive = false;
        if (transform.root.gameObject.GetComponent<Player>() != null)
        {
            player = transform.root.gameObject.GetComponent<Player>();
            //Currentcharges = MaxCharges;
            if (player.AbilityUI != null)
            {
            player.AbilityUI.transform.GetChild(0).GetComponent<Image>().color = GameManager.Instance.CurrentEnergy;
                player.AbilityUI.GetComponent<Image>().sprite = icon;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Q) && !LockAbility && Time.time > Timestamp)
        {
            //ActivateAbility();
            IsActive = true;
            TPIndicator = Instantiate(gameObject);
            GameObject FX = Instantiate(CloneEffectsPrefab, TPIndicator.transform);
            TimeManager.Instance.SetTime(0.4f);
        }

        if (Input.GetKey(KeyCode.Q) && IsActive)
        {
            if (CurrentDist < MaxDistance)
            {
                CurrentDist += 0.1f;
            }
            TPIndicator.transform.position = transform.root.position + new Vector3(0, 0, CurrentDist);
            TPIndicator.transform.rotation = transform.rotation;
        }

        if (Input.GetKeyUp(KeyCode.Q) && IsActive)
        {
            TimeManager.Instance.ResetTime();
            transform.root.position = TPIndicator.transform.position;
            IsActive = false;
            Destroy(TPIndicator);
            CurrentDist = 0;
            SetCooldown(Cooldown);
        }

        if (Time.time < Timestamp) {
            player.AbilityUI.transform.GetChild(0).GetComponent<Image>().fillAmount = (Timestamp - Time.time) / Cooldown; 
        }
	}
}
