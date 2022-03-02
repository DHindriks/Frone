using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityBombardment : AbilityBase {
    float Cooldown = 15f;
    float Timestamp;
    bool IsActive;
    bool despawning;

    float direction;
    [SerializeField]
    GameObject BomberPrefab;

    GameObject Bomber;

    //(De)ActivateAbility is called by Player.cs
    void ActivateAbility()
    {
        if (IsActive == false && Time.time > Timestamp)
        {
            IsActive = true;
            Bomber = Instantiate(BomberPrefab);
            Bomber.transform.position = new Vector3(0, 20, this.gameObject.transform.position.z - 5);
            Bomber.GetComponentInChildren<ParticleSystem>().startColor = GameManager.Instance.CurrentEnergy;
            CoolDown();
            Invoke("DespawnBomber", 4);
        }
    }

    void DeactivateAbility()
    {

    }

    void DespawnBomber()
    {
        despawning = true;
        direction = Random.Range(-6, 6);
        Bomber.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
        Invoke("DestroyBomber", 5);
    }

    void DestroyBomber()
    {
        Destroy(Bomber, 10);
        despawning = false;
        IsActive = false;
    }

    void CoolDown()
    {
        Timestamp = Time.time + Cooldown; 
    }

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
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Q) && !LockAbility)
        {
            ActivateAbility();
        }

        if (Bomber != null)
        {
            Bomber.GetComponent<Rigidbody>().AddForce(0, 0, (player.speed * 2) * Time.deltaTime, ForceMode.Force);
            Bomber.transform.LookAt(Bomber.transform.position + Bomber.GetComponent<Rigidbody>().velocity);
        }
        if (despawning == true)
        {
            Bomber.GetComponent<Rigidbody>().AddForce((player.speed * direction) * Time.deltaTime, (player.speed * 2) * Time.deltaTime, 0, ForceMode.Force);
        }
        if (Time.time < Timestamp) {
            player.AbilityUI.transform.GetChild(0).GetComponent<Image>().fillAmount = (Timestamp - Time.time) / Cooldown; 
        }
	}
}
