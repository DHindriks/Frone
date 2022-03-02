using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPInteractableBase : MonoBehaviour {

    [SerializeField]
    GameObject InteractPosition;

    [SerializeField]
    GameObject InteractView;

    [SerializeField]
    GameObject InteractUI;

    [SerializeField]
    Button FirstSelected;

    bool Interacting;
    Coroutine Moving;

    Coroutine HoloAnim;
    Vector3 Holoscale;

    void Start()
    {
        Holoscale = InteractUI.transform.localScale;
        if (InteractUI.activeSelf)
        {
            if (HoloAnim != null)
            {
                StopCoroutine(HoloAnim);
            }
            HoloAnim = StartCoroutine(DeactivateHolo(InteractUI));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FPPlayer")
        {
            other.GetComponent<FirstPersonController>().SetInteract(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "FPPlayer")
        {
            other.GetComponent<FirstPersonController>().SetInteract(null);
        }
    }

    public void Interact(GameObject other, bool interact)
    {
        Interacting = !interact;
        if (!Interacting)
        {
            other.GetComponent<FirstPersonController>().ToggleInteract(true);
            other.GetComponent<FirstPersonController>().ToggleCursorLock(false);
            Interacting = true;
            if (Moving != null)
            {
                StopCoroutine(Moving);
            }
            Moving = StartCoroutine(MoveplayerToPos(other.gameObject, 1));

            if (HoloAnim != null)
            {
                StopCoroutine(HoloAnim);
            }
            HoloAnim = StartCoroutine(ActivateHolo(InteractUI));
        }
        else
        {
            if (Moving != null)
            {
                StopCoroutine(Moving);
            }

            if (HoloAnim != null)
            {
                StopCoroutine(HoloAnim);
            }
            HoloAnim = StartCoroutine(DeactivateHolo(InteractUI));

            other.GetComponent<FirstPersonController>().ToggleCursorLock(true);
            other.GetComponent<FirstPersonController>().ToggleInteract(false);
            Interacting = false;
            }
        
    }


    IEnumerator ActivateHolo(GameObject gameObject)
    {
        float Rate = 2;
        float i = 0;

        gameObject.SetActive(true);
        if (FirstSelected != null)
        {
            FirstSelected.Select();
        }

        while (i < 1)
        {
            i += Time.deltaTime * Rate;
            gameObject.transform.localScale = Vector3.Lerp(Vector3.zero, Holoscale, i);
            yield return 0;
        }
    }

    IEnumerator DeactivateHolo(GameObject gameObject)
    {
        float Rate = 2;
        float i = 0;

        while (i < 1)
        {
            i += Time.deltaTime * Rate;
            gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, Vector3.zero, i);
            yield return 0;
        }
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        gameObject.SetActive(false);
    }

    IEnumerator MoveplayerToPos(GameObject playerobj, float LerpTime)
    {
        float Rate = 1.0f / LerpTime;
        float i = 0;
        float distance = Vector3.Distance(playerobj.transform.position, InteractPosition.transform.position);
        float StartTime = Time.time;
        Quaternion TargetCameraRotation;
        Quaternion TargetPlayerRotation;
        GameObject Cam = playerobj.transform.GetChild(1).gameObject;
        Cam.transform.position = playerobj.transform.GetChild(0).position;
        Cam.transform.rotation = playerobj.transform.GetChild(0).rotation;
        Cam.GetComponent<Camera>().depth = 1;

        while (i < 1)
        {
            i += Time.deltaTime * Rate;
            Cam.transform.position = Vector3.Lerp(playerobj.transform.position, InteractPosition.transform.position, i * 2);
            
            //rotate camera X
            TargetCameraRotation = Quaternion.LookRotation(InteractView.transform.position - Cam.transform.position, transform.TransformDirection(Vector3.up));
            Cam.transform.rotation = Quaternion.Slerp(Cam.transform.rotation, TargetCameraRotation, i);
            Cam.transform.Rotate(Cam.transform.localRotation.x , 0, 0);

            //rotate camera Y
            TargetCameraRotation = Quaternion.LookRotation(InteractView.transform.position - Cam.transform.position, transform.TransformDirection(Vector3.up));
            Cam.transform.rotation = Quaternion.Slerp(Cam.transform.rotation, TargetCameraRotation, i);
            Cam.transform.Rotate(0, Cam.transform.localRotation.y, 0);

            ////rotate player Y
            //TargetPlayerRotation = Quaternion.LookRotation(InteractView.transform.position - playerobj.transform.position, transform.TransformDirection(Vector3.up));
            //playerobj.transform.rotation = Quaternion.Slerp(playerobj.transform.rotation, TargetPlayerRotation, i);
            yield return 0;
        }
        //playerobj.transform.position = InteractPosition.transform.position;
        //Debug.Log(playerobj.transform.GetChild(0).localRotation.x);
        playerobj.GetComponent<FirstPersonController>().ResetCamRot();
    }

}
