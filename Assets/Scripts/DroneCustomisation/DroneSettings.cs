using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSettings : MonoBehaviour {

    /// <summary>
    /// This script should be added to every skin  object, This will manage a drones description, particle colors and other effects.
    /// </summary>
    //saved data
    public int UnlockState;
    public bool AppearAsPreview = true;
    public Color PrimaryColor;
    public Color SecondaryColor;
    public Color DetailsColor;
    public Color EnergyColor;

    //information data
    [SerializeField, TextArea]
    string Description;
    public Sprite SelectorIcon;

    //stats
    public float BoostGlide;
    public float BoostMultiplier = 1;
    public float GravityMultiplier = 1;

    //particles and anims
    public ParticleSystem ImpactParticles;

    [SerializeField]
    ParticleSystem DeathExp;

    [SerializeField]
    GameObject EnergyShieldPrefab;

    [SerializeField]
    List<ParticleSystem> DroneParticles = new List<ParticleSystem>();

    [SerializeField, Tooltip("For lights that use picked energy color")]
    List<Light> DroneLights = new List<Light>();

    [SerializeField]
    List<ParticleSystem> BoostParticles = new List<ParticleSystem>();

    [SerializeField]
    List<ParticleSystem> OverheatParticles = new List<ParticleSystem>();

    [SerializeField]
    List<GameObject> DeathObjects = new List<GameObject>();

    [SerializeField]
    List<ParticleSystem> DeathParticles = new List<ParticleSystem>();
    Animator anim;

    [HideInInspector]
    public GameObject Energyshield;

    // Use this for initialization
    void Awake() {
        Invoke("SetUp", 0.001f);
        if (EnergyShieldPrefab != null)
        {
            Energyshield = Instantiate(EnergyShieldPrefab, transform.root);
        }
    }

    void SetUp()
    {
        if (GetComponent<Animator>() != null)
        {
            anim = GetComponent<Animator>();
        }
        if (EnergyShieldPrefab != null)
        {
            Energyshield.GetComponent<Renderer>().material.SetColor("_color", EnergyColor);
        }
        LoadDroneData();
        SetColors(gameObject);
    }

    public void LoadDroneData()
    {
        DroneData data = SaveSystem.LoadPlayer(this);
        if (data != null)
        {
            UnlockState = data.Unlocked;
            ColorUtility.TryParseHtmlString("#" + data.PrimaryColor, out PrimaryColor);
            ColorUtility.TryParseHtmlString("#" + data.SecondaryColor, out SecondaryColor);
            ColorUtility.TryParseHtmlString("#" + data.DetailColor, out DetailsColor);
            ColorUtility.TryParseHtmlString("#" + data.EnergyColor, out EnergyColor);
        }

    }

    public void SaveDroneData()
    {
        SaveSystem.SaveDrone(this);
    }
    void SetParticleColors(List<ParticleSystem> list)
    {
        foreach (ParticleSystem particles in list)
        {
            foreach(Material mat in particles.GetComponent<ParticleSystemRenderer>().materials)
            {
                //if (particles.GetComponent<ParticleSystemRenderer>().material.name.Contains("DroneEnergy"))
                //{
                    particles.startColor = EnergyColor;
                    mat.SetColor("_Color", EnergyColor);
                    mat.SetColor("_EmissionColor", EnergyColor);
                //}
            }

            if (particles.GetComponent<ParticleSystemRenderer>().trailMaterial != null)
            {
                if (particles.GetComponent<ParticleSystemRenderer>().trailMaterial.name.Contains("DroneEnergy"))
                {
                    particles.GetComponent<ParticleSystemRenderer>().trailMaterial.SetColor("_Color", EnergyColor);
                    particles.GetComponent<ParticleSystemRenderer>().trailMaterial.SetColor("_EmissionColor", EnergyColor);
                }
            }

            foreach (Light light in DroneLights)
            {
                light.color = EnergyColor;
            }
        }
    }

    public void SetColors(GameObject ParentObj)
    {
        Transform[] AllChildren = ParentObj.GetComponentsInChildren<Transform>(); 
        foreach (Transform child in AllChildren)
        {
            if (child.GetComponent<Renderer>() != null)
            {
                Material RenderedMat = child.GetComponent<Renderer>().material;
                if (RenderedMat.name.Replace(" (Instance)", "") == "DronePrimary")
                {
                    RenderedMat.SetColor("_Color", PrimaryColor);
                    RenderedMat.SetColor("_EmissionColor", PrimaryColor);
                }else if (RenderedMat.name.Replace(" (Instance)", "") == "DroneSecondary")
                {
                    RenderedMat.SetColor("_Color", SecondaryColor);
                    RenderedMat.SetColor("_EmissionColor", SecondaryColor);
                }
                else if (RenderedMat.name.Replace(" (Instance)", "") == "DroneDetails")
                {
                    RenderedMat.SetColor("_Color", DetailsColor);
                    RenderedMat.SetColor("_EmissionColor", DetailsColor);
                }
                else if (RenderedMat.name.Replace(" (Instance)", "") == "DroneEnergy")
                {
                    RenderedMat.SetColor("_Color", EnergyColor);
                    RenderedMat.SetColor("_EmissionColor", EnergyColor);
                }
            }

            if (child.GetComponent<TrailRenderer>() != null)
            {
                foreach (Material mat in child.GetComponent<TrailRenderer>().materials)
                {
                    mat.SetColor("_Color", EnergyColor);
                    mat.SetColor("_EmissionColor", EnergyColor);
                }
            }

            SetParticleColors(DroneParticles);
            SetParticleColors(BoostParticles);
            if (!name.Contains("Frone_Dropship"))
            {
                GameManager.Instance.CurrentPrimary = PrimaryColor;
                GameManager.Instance.CurrentSecondary = SecondaryColor;
                GameManager.Instance.CurrentDetails = DetailsColor;
                GameManager.Instance.CurrentEnergy = EnergyColor;
            }
        }
    }

    public void ToggleBoost(bool boosting)
    {
        if (anim != null)
        {
            anim.SetBool("Boosting", boosting);
        }

        foreach (ParticleSystem particles in BoostParticles)
        {
            if (boosting == true)
            {
                particles.Play();
            }
            else
            {
                particles.Stop();
            }
        }
    }

    public void ToggleOverheat(bool Overheating)
    {
        foreach (ParticleSystem particles in OverheatParticles)
        {
            if (Overheating == true)
            {
                particles.Play();
            }
            else
            {
                particles.Stop();
            }
        }
    }

    public void Explode()
    {
        foreach (GameObject deathobject in DeathObjects)
        {
            deathobject.transform.parent = null;
            deathobject.AddComponent<MeshCollider>().convex = true;
            deathobject.AddComponent<Rigidbody>().mass = 0.25f;
            deathobject.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(25, 400), transform.root.position, 10);
            //StartCoroutine(ResetRigidbody(deathobject.GetComponent<Rigidbody>(), deathobject));

        }

        foreach(ParticleSystem particles in DroneParticles)
        {
            particles.Stop();
        }

        foreach(ParticleSystem particles in DeathParticles)
        {
            particles.Play();
        }

        if (DeathExp != null)
        {
            GameObject Explosion = Instantiate(DeathExp.gameObject);
            Explosion.transform.position = transform.position;
        }

    }

    private IEnumerator ResetRigidbody(Rigidbody rigidbody, GameObject src)
    {
        Destroy(rigidbody);
        yield return null;
        src.AddComponent<Rigidbody>();
        Rigidbody rb = src.GetComponent<Rigidbody>();
        rb.drag = 2;
        rb.angularDrag = 0.75f;
    }
}
