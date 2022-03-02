using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageFloor : MonoBehaviour {
    public GameObject target;

    public Color FloorColor;

    void Start()
    {
        SetReference();
    }

    void SetReference()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.FloorManager == null)
            {
                GameManager.Instance.FloorManager = this;
            } 
        }else
        {
            Invoke("SetReference", 0.1f);
        }
    }

    // Update is called once per frame
    void Update () {
        if (target != null)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(target.transform.position.x, -1.5f, target.transform.position.z), 10f * Time.deltaTime);
        }

        this.GetComponent<Renderer>().material.SetColor("_color", Color.Lerp(this.GetComponent<Renderer>().material.GetColor("_color"), FloorColor, 0.01f));

    }
}
