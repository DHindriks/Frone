using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour {
    [HideInInspector]
	public float speed;
    public float Startingspeed = 750;
    public float Acceleration = 4;

    //gameover vars
    [HideInInspector]
    public bool gameover;
    [HideInInspector]
    public bool Checkspeed = true; //if sudden speed changes should count as a crash, often disabled by events like cutscenes
    [HideInInspector]
    public bool EnergyShieldActive = true; //player crash as long as the energyshield is active.
    public bool EnergyShieldCooldown = false; //is energy shield on cooldown
    GameObject EnergyShieldObj;
    [HideInInspector]
    public bool Undying = false;  //A second chance, just like the energyshield, but this one is managed by abilities.

    [HideInInspector]
    public AbilityBase ability;
    [HideInInspector]
    public DroneSettings droneSettings;
    [HideInInspector]
    public GameObject ImpactParticles;
    float flyheight = 1.5f;

    public float ScoreMultiplier;
    [SerializeField]
    List<Transform> hoverpoints = new List<Transform>();
    Vector3 Gravity;
    float originalflyheight;
    public GameObject Spawnmanager;
    GameObject ModelObject;
    Rigidbody playerrb;
    Collider playerCol;
    Vector3 Angle;
    float PreviousVelocityMag;
    bool grounded; // is player close enough to ground to recharge fuel

    //Roll function
    bool Rolling;
    KeyCode LastPress;
    float InputTime;

    // boost related vars
    public float MaxFuel;
    public float fuel;
    bool isboosting;
    bool FuelRegenerating;
    bool BoostOverheat;
    Coroutine LastBoostDelay;

    public GameObject fuelbar;

    public GameObject AbilityUI;

    public GameObject cam;
    Camerascript CamScript;

    public bool Allowcontrols;
    public bool Invincible; //if toggled true, player won't die when flying to slow or colliding with dangerous objects. Falling still kills, however
    
//TODO: add more abilities
/// <summary>
/// Ability ideas:
/// 
/// destabilize: variable/medium cooldown, allows player to phase through obstacles.
/// 
/// bombardment: high cooldown, destroys obstacles in front of the player.
/// 
/// Turret: variable/high cooldown, destroys nearby enemies.
/// 
/// Overclock: variable/low cooldown, overclocks drone processing power, allowing it to register the world a lot faster, making it look like the player is in slow motion. It's actually slow motion in code btw 
/// 
/// </summary>
    //game over vars
    [SerializeField]
    GameObject gameoverscreen;
    public GameObject MissionOverScreen;

    [SerializeField]
    ObjectiveManager Scoremanager;

    [SerializeField]
    Text score;

    //inventory
    Inventory inventory;

    // Use this for initialization
    void Awake () {
        GameManager.Instance.player = this.gameObject;
        GameManager.Instance.Setplayerskin();
        originalflyheight = flyheight;
        speed = Startingspeed;
        playerrb = GetComponent<Rigidbody>();
        inventory = GetComponent<Inventory>();
        PreviousVelocityMag = playerrb.velocity.magnitude;
        playerCol = GetComponent<Collider>();
        droneSettings = GetComponentInChildren<DroneSettings>();
        ability = GetComponentInChildren<AbilityBase>();
        ability.LockAbility = false;
        ModelObject = transform.GetChild(0).gameObject;
        EnergyShieldObj = droneSettings.Energyshield;
        CamScript = cam.GetComponent<Camerascript>();
        Angle = ModelObject.transform.localRotation.eulerAngles;
        MaxFuel = 10;
        fuel = MaxFuel;
        ChangeGravity();
        BoostOverheat = false;
        gameover = false;

        if (droneSettings.ImpactParticles != null)
        {
            ImpactParticles = Instantiate(droneSettings.ImpactParticles.gameObject);
        }

        if (fuelbar != null)
        {
            fuelbar.GetComponent<Image>().color = GameManager.Instance.CurrentEnergy;
        }
    }

    void FixedUpdate()
    {
        playerrb.AddForce(((Gravity * playerrb.mass) * (speed / 10)) * droneSettings.GravityMultiplier * Time.deltaTime);
        foreach (Transform hoverpoint in hoverpoints)
        {
            RaycastHit Hit;
            Ray distancetoground = new Ray(hoverpoint.position, Gravity);
            if (Physics.Raycast(distancetoground, out Hit))
            {
                if (Hit.distance < flyheight)
                {
                    playerrb.AddForceAtPosition((-Gravity * playerrb.mass) * (speed / (Startingspeed / 5f) * (flyheight - Hit.distance) * 2) / hoverpoints.Count, hoverpoint.position, ForceMode.Force);
                    ScoreMultiplier = 1;
                    grounded = true;
                }

                if (Hit.distance > originalflyheight * 1.5f)
                {
                    ScoreMultiplier = 2;
                    grounded = false;

                }
                else if (Hit.distance - 1 < flyheight)
                {
                    if (FuelRegenerating == true && fuel < MaxFuel && !Input.GetKey(KeyCode.S))
                    {
                        SetFuel(4f / hoverpoints.Count * Time.deltaTime);
                    }
                    else if (fuel >= 10 && BoostOverheat)
                    {
                        BoostOverheat = false;
                        droneSettings.ToggleOverheat(false);
                        fuel = 10;
                        Debug.Log("COOLED");
                    }
                }
            }
        }

    }

    // Update is called once per frame
    void Update () {
        //gravity default is -9.8 for earth
        
        if (fuelbar != null && Allowcontrols)
        {
            fuelbar.transform.localScale = new Vector3(0.75f, fuel / MaxFuel, 0.75f);
        }
        
        
        if (gameover == false)
        {
            playerrb.AddForce(new Vector3(0, 0, speed) * Time.deltaTime, ForceMode.Force);
            //transform.LookAt(transform.position + playerrb.velocity + new Vector3(0, 0, 5));
        }


        if (this.gameObject.transform.position.y <= -10 && gameover == false)
        {
            GameOver(true);
        }


        //TODO: Finish android traces or remove them, at this point, they will probably be removed.
        //left and right steering on android
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];

            if (touch.position.x <= Screen.width / 2)
            {
                GoLeft();
            }

            if (touch.position.x >= Screen.width / 2)
            {
                GoRight();
            }

        }



        //Boost and Dive
        if (Input.GetKey(KeyCode.S) && Allowcontrols == true && !BoostOverheat)
        {
            Dive();  
        } else if (Input.GetKeyUp(KeyCode.S))
        {
            flyheight = originalflyheight;
        }

        if (Input.GetKey(KeyCode.W) && Allowcontrols == true && !BoostOverheat)
        {
            if (isboosting == false)
            {
                droneSettings.ToggleBoost(true);
                CamScript.ChangeZoom(80, 1);
            }
            Boost();
        } else if (Input.GetKeyUp(KeyCode.W))
        {
            StopBoost();
        }

        //TODO: Add some kind of benefit from flipping, (small boost in flip direction? Bullet reflect mechanic? gravity rolling mechanic?)
        //Flips
        if (Input.GetKeyDown(KeyCode.A) && Allowcontrols == true)
        {
            if (!Rolling && LastPress == KeyCode.A && InputTime + 0.25f >= Time.time)
            {
                //do flip
                StartCoroutine("FlipHorizontal", 360);
                Rolling = true;
                InputTime = Time.time;

            }
            else
            {
                //no flip
                LastPress = KeyCode.A;
                InputTime = Time.time;
            }
        }
        if (Input.GetKeyDown(KeyCode.D) && Allowcontrols == true)
        {
            if (!Rolling && LastPress == KeyCode.D && InputTime + 0.25f >= Time.time)
            {
                //do flip
                StartCoroutine("FlipHorizontal", -360);
                Rolling = true;
                InputTime = Time.time;

            }
            else
            {
                //no flip
                LastPress = KeyCode.D;
                InputTime = Time.time;
            }
        }



        //left right steering
        if (Input.GetKey(KeyCode.A) && Allowcontrols == true)
        {
            GoLeft();
            Angle.z = 20;
            Angle.y = -20;
        }
        if (Input.GetKey(KeyCode.D) && Allowcontrols == true)
        {
            GoRight();
            Angle.z = -20;
            Angle.y = 20;
        }

        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D) || !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            Angle.z = 0;
            Angle.y = 0;
        }
        if (!Rolling)
        {
            ModelObject.transform.localEulerAngles = new Vector3(0, Mathf.LerpAngle(0, Angle.y, 10 * Time.deltaTime), Mathf.LerpAngle(ModelObject.transform.localEulerAngles.z, Angle.z, 10 * Time.deltaTime));
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, Mathf.LerpAngle(transform.localEulerAngles.y, 0, 10 * Time.deltaTime), transform.localEulerAngles.z);
        }else
        {
            Quaternion q = Quaternion.FromToRotation(transform.up, -Gravity) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 5f);
        }

        //player dies if not moving
        //TODO: Improve this. Player shouldn't die when velocity is too low, player should die if there is a sudden loss of velocity.
        //if (playerrb.velocity.z <= 1 && gameover == false && Checkspeed == true && Invincible == false)
        //{
        //    GameOver();
        //}

        if (PreviousVelocityMag - playerrb.velocity.magnitude > 10 && !gameover && Checkspeed && !Invincible)
        {
            GameOver(true);
        }
        //Debug.Log(playerrb.velocity.magnitude + " " + PreviousVelocityMag);
        PreviousVelocityMag = playerrb.velocity.magnitude;

    }

    IEnumerator FlipHorizontal(float Degrees)
    {
        float startRotation = ModelObject.transform.localEulerAngles.y;
        float endRotation = startRotation + Degrees;
        float t = 0.0f;
        float duration = 0.5f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float zRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            ModelObject.transform.localEulerAngles = new Vector3(0, 0, zRotation);
            yield return null;
        }
        Rolling = false;
    }


    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "SpawnTrigger")
        {
            GameManager.Instance.SpawnManager.SpawnNewTile();
            GameManager.Instance.SpawnManager.OldTile = col.transform.root.gameObject;
        }
    }


    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Danger" && Invincible == false)
        {
            GameOver();
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (ImpactParticles != null)
        {

            Vector3 impactpos = new Vector3();
            foreach(ContactPoint point in collision.contacts)
            {
                impactpos += point.point;
            }
            impactpos = Vector3.Lerp(impactpos / collision.contacts.Length, playerCol.ClosestPoint(collision.transform.position), 0.5f);

            ImpactParticles.transform.position = impactpos;
            ImpactParticles.transform.rotation = transform.GetChild(0).rotation;
            ImpactParticles.GetComponent<ParticleSystem>().Play();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (ImpactParticles != null)
        {
            ImpactParticles.GetComponent<ParticleSystem>().Stop();
        }
    }

    public void GoRight()
    {
        playerrb.AddForce(transform.TransformDirection(Vector3.right) * speed * Time.deltaTime, ForceMode.Acceleration);
        //playerrb.AddForce(new Vector3(speed, 0, 0) * Time.deltaTime, ForceMode.Acceleration);
    }

    public void GoLeft()
    {
        playerrb.AddForce(transform.TransformDirection(Vector3.left) * speed * Time.deltaTime, ForceMode.Acceleration);
        //playerrb.AddForce(new Vector3(-speed, 0, 0) * Time.deltaTime, ForceMode.Acceleration);
    }

    //TODO: Show reputation gain and loss in gameover screen.
    void GameOver(bool Truedeath = false)
    {
        if (!Undying && !EnergyShieldActive || Truedeath)
        {
            foreach (Transform child in gameoverscreen.transform.root)
            {
                child.gameObject.SetActive(false);
            }
            gameover = true;
            ability.LockAbility = true;
            StopBoost();
            gameObject.GetComponent<BoxCollider>().enabled = false;
            playerrb.isKinematic = true;
            gameObject.GetComponent<Rigidbody>().freezeRotation = false;
            droneSettings.Explode();
            cam.GetComponent<Camerascript>().SetcamPos(transform.GetChild(Random.Range(1, 2)).gameObject, this.gameObject, 0, 0, 5, 2);
            Allowcontrols = false;

            if (ImpactParticles != null)
            {
                ImpactParticles.GetComponent<ParticleSystem>().Stop();
            }

            foreach (StackItem item in inventory.ItemList)
            {
                item.amount = Mathf.FloorToInt(item.amount / 2);
            }

            Invoke("GameOverPhase2", 4);
        }else if (EnergyShieldActive)
        {
            EnergyBurst();
        }
    }

    void GameOverPhase2 ()
    {
        int Finalscore = Mathf.FloorToInt(Scoremanager.Score);
        string Supportingfaction = PlayerPrefs.GetString("SupportingFaction", null);

        GameManager.Instance.AddReputation(Supportingfaction, Finalscore / 10);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameoverscreen.SetActive(true);
        score.text = "Score: " + Finalscore.ToString();
        gameoverscreen.transform.position = Vector3.Lerp(gameoverscreen.transform.position,gameoverscreen.transform.root.position, 5);
    }

    public void EndlevelPhase2()
    {
        MissionOverScreen.SetActive(true);
        inventory.TransferInventory(GameManager.Instance.MainInventory);
        GameManager.Instance.MainInventory.SaveInventory();
    }

    public void FreezePlayer(bool freeze)
    {
        Invincible = freeze;
        Allowcontrols = !freeze;
        playerrb.isKinematic = freeze;
        ability.LockAbility = freeze;
        Checkspeed = !freeze;
    }

    public void ChangeGravity(float X = 0, float Y = -9.8f, float Z = 0)
    {
        Gravity = new Vector3(X, Y, Z);
        var q = Quaternion.FromToRotation(transform.up, -Gravity);
        transform.rotation = q * transform.rotation;
    }

    //player dive
    public void Dive()
    {
        if (fuel > 0.1)
        {
            flyheight = 0.05f;
            SetFuel(-3f * Time.deltaTime);
            playerrb.AddRelativeForce(new Vector3(0, 0, -speed / 2) * Time.deltaTime, ForceMode.Force);
        } else
        {
                        BoostOverheat = true;
            droneSettings.ToggleOverheat(true);
            Debug.Log("OVERHEATING");
            StopBoost();
            flyheight = originalflyheight;
        }
    }

        //player boost
    public void Boost()
    {
        if (fuel > 0)
        {
            playerrb.AddRelativeForce(new Vector3(0, 0, 20) * droneSettings.BoostMultiplier * Time.deltaTime, ForceMode.Impulse);
            //playerrb.AddForce(0, playerrb.mass * droneSettings.BoostGlide , 0, ForceMode.Force);
            playerrb.AddForce(((Physics.gravity * playerrb.mass) * -(speed / 10)) * droneSettings.BoostGlide * Time.deltaTime);
            isboosting = true;
            FuelRegenerating = false;
            SetFuel(-1.5f * Time.deltaTime);
        } else
        {
            BoostOverheat = true;
            droneSettings.ToggleOverheat(true);
            Debug.Log("OVERHEATING");
            StopBoost();
        }
    }

    void StopBoost()
    {
        isboosting = false;
        droneSettings.ToggleBoost(false);
        CamScript.ChangeZoom(60, 1.5f);
        if (LastBoostDelay != null)
        {
            StopCoroutine(LastBoostDelay);
        }
        LastBoostDelay = StartCoroutine(BoostDelay(0.75f));
    }

    void SetShield(bool active = true)
    {
        EnergyShieldActive = active;

        if (EnergyShieldActive)
        {
            EnergyShieldObj.SetActive(true);
            EnergyShieldCooldown = false;
        }else
        {
            EnergyShieldObj.SetActive(false);
        }
    }

    void EnergyBurst ()
    {
        SetShield(false);
        EnergyShieldCooldown = true;

    }
 
    void SetFuel(float ToAdd, bool set = false)
    {
        if(!set)
        {
            if (fuel + ToAdd > MaxFuel)
            {
                fuel = MaxFuel;
                BoostOverheat = false;
            }else if (fuel + ToAdd < 0)
            {
                fuel = 0;
            }else
            {
                fuel += ToAdd;
            }
        }else
        {
            fuel = ToAdd;
        }

        if (fuel < MaxFuel / 3)
        {
            SetShield(false);
        }else if (fuel >= MaxFuel / 3 && !EnergyShieldCooldown && !BoostOverheat)
        {
            SetShield(true);
        }
    }

    //waits for seconds after player stops boosting to start recharging again.
    IEnumerator BoostDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        FuelRegenerating = true;
    }
}
