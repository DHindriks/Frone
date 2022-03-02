using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour {

    [SerializeField]
    GameObject SelectorCamera;

    [SerializeField]
    GameObject SelectorTexture;

    [SerializeField]
    Transform SelectorContainer;

    [SerializeField]
    GameObject ButtonPrefab;

    [SerializeField]
    GameObject PlayerContainer;

    [SerializeField]
    ColorpickerUI[] ui;

    [SerializeField]
    GameObject Lockparticles;


    Object[] SkinList;
    Vector3 Startpos = new Vector3(0, 20, 0);
    GameObject SkinObject;
    Vector3 CamLerpPos = new Vector3(0, 20, 5);

	// Use this for initialization
	void Awake ()
    {
        Invoke("SetUp", 0.1f);
    }

    void SetUp()
    {
        SkinList = Resources.LoadAll("Drones", typeof(GameObject));
        if (SelectorTexture != null)
        {
            foreach (GameObject Skin in SkinList)
            {
                SkinObject = Instantiate(Skin);
                SkinObject.transform.position = Startpos;
                Startpos.x += 3;
                SkinObject.layer = 8;
                SkinObject.AddComponent<SphereCollider>().radius = 1.5f;
                SetLayerRecursively(SkinObject);

                SkinObject.transform.Rotate(new Vector3(10, 20, 0));
            }
        }else if (SelectorContainer != null)
        {
            foreach (GameObject skin in SkinList)
            {
                Sprite SkinIcon = null;
                string SkinName = null;
                if (skin.GetComponent<DroneSettings>() != null)
                {
                    DroneSettings SkinSettings = skin.GetComponent<DroneSettings>();
                    if (SkinSettings.AppearAsPreview)
                    {
                        DroneData droneData = SaveSystem.LoadPlayer(SkinSettings);
                        bool preview = false;
                        if (droneData != null && droneData.Unlocked != 1)
                        {
                            preview = true;
                        }
                        if (SkinSettings.SelectorIcon != null) 
                        SkinIcon = SkinSettings.SelectorIcon;
                        SkinName = skin.name;
                        GameObject button = Instantiate(ButtonPrefab, SelectorContainer);
                        if (SkinIcon != null)
                        button.transform.GetChild(0).GetComponent<Image>().sprite = SkinIcon;
                        button.name = SkinName;
                        button.GetComponent<Button>().onClick.AddListener(delegate { ClickSelector(SkinName, preview); });
                        button.transform.GetChild(1).GetComponentInChildren<Text>().text = SkinName;
                    }
                  
                }
            }
        }

        GameManager.Instance.Setplayerskin();
        foreach (ColorpickerUI ui in ui)
        {
            ui.Invoke("ResetColors", .01f);
        }
    }

    void SetLayerRecursively(GameObject gameObject)
    {
        gameObject.layer = 8;
        foreach(Transform child in gameObject.transform)
        {
            SetLayerRecursively(child.gameObject);
        }
    }


    public void ClickSelector(string SkinName = null, bool Preview = false)
    {
        if (SkinName != null)
        {
            Debug.Log(Preview);
            if (Resources.Load("Drones/" + SkinName) != null)
            {
                GameManager.Instance.Setplayerskin(SkinName);
                if (!Preview)
                {

                    if (Lockparticles != null)
                    {
                        Lockparticles.GetComponentInChildren<ParticleSystem>().Stop();
                    }

                    PlayerPrefs.SetString("Model", SkinName);
                }
                else
                {
                    Lockparticles.GetComponentInChildren<ParticleSystem>().Play();
                    Lockparticles.transform.position = PlayerContainer.transform.position;
                    Debug.Log(SkinName + " has not been unlocked yet");
                }
            }
            foreach (ColorpickerUI ui in ui)
            {
                ui.Invoke("ResetColors", .01f);
            }
        }

    }

    private void Update()
    {
        if (SelectorCamera.transform.position.x != CamLerpPos.x)
        {
        SelectorCamera.transform.position = Vector3.Lerp(SelectorCamera.transform.position, CamLerpPos, 0.05f);
        }

    }

}
