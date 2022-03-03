using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camerascript : MonoBehaviour {

    public GameObject FollowTarget;
    public GameObject LookTarget;

    float PosLerpSpeed = 10;
    float LookLerpspeed = 1f;

    Quaternion TargetRotation;
    Coroutine LastRoutine;

    Camera MainCamera;
    [SerializeField]
    GameObject playerobj;
    float AxisLerpamount;
    float LookLerpamount;

    // Use this for initialization
    void Start () {
        MainCamera = gameObject.GetComponentInChildren<Camera>();
        GameManager.Instance.cam = this;
        SetcamPos();
    }
	
    public void SetcamPos(GameObject Postarget = null, GameObject Lookttarget = null, float Axislerp = 0.75f, float LookLerp = 0.5f, float PosSpeed = 10f, float RotSpeed = 2f)
    {
        if (Postarget == null)
        {
            Postarget = playerobj;
        }
        if (Lookttarget == null)
        {
            Lookttarget = playerobj;
        }
        FollowTarget = Postarget;
        LookTarget = Lookttarget;
        AxisLerpamount = Axislerp;
        LookLerpamount = LookLerp;
        PosLerpSpeed = PosSpeed;
        LookLerpspeed = RotSpeed;
    }
    
    public void ChangeZoom(float FOV = 60, float LerpTime = 2)
    {
        if (LastRoutine != null)
        {
            StopCoroutine(LastRoutine);
        }
        LastRoutine = StartCoroutine(LerpZoom(FOV, LerpTime));
    }

    IEnumerator LerpZoom(float FOV, float LerpTime)
    {
        float Rate = 1.0f / LerpTime;
        float i = 0;

        while (i < 1)
        {
            i += Time.deltaTime * Rate;
            MainCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, FOV, i);
            yield return 0;
        }

        MainCamera.fieldOfView = FOV;
    }

        Vector3 positionVelocity = Vector3.zero;
    // Update is called once per frame
    void FixedUpdate () {

        Vector3 TargetPos = new Vector3(Vector3.Lerp(FollowTarget.transform.position, this.transform.GetChild(0).transform.localPosition, AxisLerpamount).x, FollowTarget.transform.position.y, FollowTarget.transform.position.z);
        //transform.position = Vector3.Lerp(transform.position, TargetPos, PosLerpSpeed * Time.deltaTime);
        transform.position = Vector3.SmoothDamp(transform.position, TargetPos,ref positionVelocity, 0.3f);
        TargetRotation = Quaternion.LookRotation(LookTarget.transform.position - this.transform.GetChild(0).position);
        transform.GetChild(0).rotation = Quaternion.Slerp(this.transform.GetChild(0).rotation, Quaternion.Lerp(TargetRotation, transform.rotation, LookLerpamount), LookLerpspeed * Time.deltaTime);
    }
}
