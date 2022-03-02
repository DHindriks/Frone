using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityTurret : AbilityBase {
    float MinCooldown = 2.5f;
    float VarCooldown = 7.5f;
    float CooldownTime;
    float Timestamp;
    float BeginTime;
    bool IsActive;


    [SerializeField]
    GameObject Turret;
    [SerializeField]
    List<ParticleSystem> Shooteffects = new List<ParticleSystem>();

    [SerializeField]
    GameObject Impact;

    [SerializeField]
    float FireRate = 2.5f;

    float LastFired;

    [SerializeField]
    GameObject AimCamPos;

    [SerializeField]
    GameObject CrosshairPrefab;

    GameObject Crosshair;

    
    Vector3 AimPos;


    // Use this for initialization
    void Start () {
        base.Start();
        Invoke("SetUp", 0.2f);
    }

    void SetUp()
    {
        if (transform.root.gameObject.GetComponent<Player>() != null)
        {
            AimPos = new Vector3(player.cam.GetComponentInChildren<Camera>().pixelWidth / 2, player.cam.GetComponentInChildren<Camera>().pixelHeight / 2, 0);

            if (player.AbilityUI != null)
            {
                player.AbilityUI.transform.GetChild(0).GetComponent<Image>().color = GameManager.Instance.CurrentEnergy;
                player.AbilityUI.GetComponent<Image>().sprite = icon;
            }
        }
    }

    //(De)ActivateAbility is called by Player.cs
    void ActivateAbility()
    {
        if (Time.time > Timestamp)
        {
            Setability(!IsActive);
        }
    }

    void DeactivateAbility()
    {

    }


    bool Setability(bool active)
    {
        if (active == true)
        {
            player.GetComponent<Player>().Allowcontrols = false;
            if (Crosshair != null)
            {
                Crosshair.SetActive(true);
                AimPos = new Vector3(player.cam.GetComponentInChildren<Camera>().pixelWidth / 2, player.cam.GetComponentInChildren<Camera>().pixelHeight / 2, 0);
            }else
            {
                Crosshair = Instantiate(CrosshairPrefab, player.MissionOverScreen.transform.root) as GameObject;
                Crosshair.GetComponent<RawImage>().color = GameManager.Instance.CurrentEnergy;

                foreach (ParticleSystem effect in Shooteffects)
                {
                    effect.startColor = GameManager.Instance.CurrentEnergy;
                }
            }
            TimeManager.Instance.SetTime(0.6f);
            player.cam.GetComponent<Camerascript>().FollowTarget = AimCamPos;
            player.cam.GetComponent<Camerascript>().LookTarget = AimCamPos;
            player.cam.GetComponent<Camerascript>().ChangeZoom(55);

            IsActive = true;
            BeginTime = Time.time;
        }else
        {
            player.GetComponent<Player>().Allowcontrols = true;
            player.cam.GetComponent<Camerascript>().FollowTarget = this.transform.root.gameObject;
            player.cam.GetComponent<Camerascript>().LookTarget = this.transform.root.gameObject;
            TimeManager.Instance.ResetTime();

            Crosshair.SetActive(false);
            IsActive = false;
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
            ActivateAbility();
        }

        if (IsActive == true)
        {
            Ray ray = player.cam.GetComponentInChildren<Camera>().ScreenPointToRay(AimPos);
            RaycastHit hit;
            if (player.gameover)
            {
                ActivateAbility();
            }
            if (Physics.Raycast(ray, out hit))
            {
                Turret.transform.LookAt(hit.point);


            } else
            {
                Turret.transform.LookAt(ray.GetPoint(100));
            }

            if (Time.time - LastFired > 1 / FireRate)
            {
                LastFired = Time.time;

                foreach (ParticleSystem effect in Shooteffects)
                {
                    effect.Play();
                }

                if (hit.transform != null)
                {
                    GameObject impact = Instantiate(Impact);
                    impact.transform.position = hit.point;
                    impact.transform.rotation = Quaternion.LookRotation(hit.point + Vector3.Cross(-hit.normal, transform.right), hit.normal);
                    impact.GetComponent<ParticleSystem>().startColor = GameManager.Instance.CurrentEnergy;

                    if (hit.collider.gameObject.tag == "EnergyShield")
                    {
                        //Destroy(hit.collider.gameObject);
                        StartCoroutine("ShrinkObject", hit.collider.gameObject);
                    }else if (hit.collider.gameObject.tag == "Obstacle" || hit.collider.gameObject.tag == "Danger")
                    {
                        Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.transform.SetParent(rb.transform.root);
                            rb.isKinematic = false;
                            rb.AddExplosionForce(100, hit.point, 2);
                        }
                    }
                }

            }
            Crosshair.transform.position = AimPos;
            
            if (Input.GetKey(KeyCode.W))
            {
                AimPos.y += 300 * Time.deltaTime;
            } else if (Input.GetKey(KeyCode.S))
            {
                AimPos.y -= 300 * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                AimPos.x -= 300 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                AimPos.x += 300 * Time.deltaTime;
            }


        }



        //manages cooldown / active timer in UI
        if (Time.time < Timestamp  && IsActive == false)
        {
            player.AbilityUI.transform.GetChild(0).GetComponent<Image>().fillAmount = (Timestamp - Time.time) / (MinCooldown + VarCooldown);
        } else if (IsActive == true)
        {
            player.AbilityUI.transform.GetChild(0).GetComponent<Image>().fillAmount = (Time.time - BeginTime) / VarCooldown;
            if (BeginTime + VarCooldown < Time.time)
            {
                ActivateAbility();
            }
        }


    }

    IEnumerator ShrinkObject(GameObject gameObject)
    {
        float Rate = 2;
        float i = 0;

        while (i < 1)
        {
            i += Time.deltaTime * Rate;
            gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, Vector3.zero, i);
            yield return 0;
        }
        Destroy(gameObject);
    }
}