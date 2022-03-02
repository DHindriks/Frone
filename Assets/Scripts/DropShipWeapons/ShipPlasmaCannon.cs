using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ShipPlasmaCannon : ShipWeaponBase {
    Vector3 AimPos;
    [SerializeField]
    GameObject CrosshairPrefab;
    [SerializeField]
    List<GameObject> Turrets;

    [SerializeField]
    List<ParticleSystem> ShootParticles;

    bool SetUpComplete;

    GameObject Crosshair;

    // Use this for initialization
    void Start () {

        //Invoke("SetUp", 0.2f);
        SetUp();
    }

    void SetUp()
    {
        base.Start();
        if (Crosshair != null)
        {
            Crosshair.SetActive(true);
            AimPos = new Vector3(AttackBase.player.cam.GetComponentInChildren<Camera>().pixelWidth / 2, AttackBase.player.cam.GetComponentInChildren<Camera>().pixelHeight / 2, 0);
        }
        else
        {
            Crosshair = Instantiate(CrosshairPrefab, AttackBase.player.MissionOverScreen.transform.root) as GameObject;
            Crosshair.GetComponent<RawImage>().color = GameManager.Instance.CurrentEnergy;
        }

        AimPos = new Vector3(AttackBase.player.cam.GetComponentInChildren<Camera>().pixelWidth / 2, AttackBase.player.cam.GetComponentInChildren<Camera>().pixelHeight / 2, 0);
        SetUpComplete = true;
    }

    // Update is called once per frame
    void Update () {

        if (SetUpComplete)
        {

            Ray ray = AttackBase.player.cam.GetComponentInChildren<Camera>().ScreenPointToRay(AimPos);
            RaycastHit hit;

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Fire();
            }

            if (Physics.Raycast(ray, out hit))
            {
                foreach (GameObject Turret in Turrets)
                {
                    Turret.transform.LookAt(hit.point);
                }
            }
            else
            {
                foreach (GameObject Turret in Turrets)
                {
                    Turret.transform.LookAt(ray.GetPoint(500));
                }
            }

            //Crosshair controls
            Crosshair.transform.position = AimPos;
            if (Input.GetKey(KeyCode.W))
            {
                AimPos.y += 300 * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.S))
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
    }

    void Fire()
    {
        foreach (ParticleSystem part in ShootParticles)
        {
            part.Play();
        }
    }
}
