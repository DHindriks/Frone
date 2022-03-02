using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamChangeZone : MonoBehaviour {

    Camerascript cam;

    [SerializeField]
    bool Autoreset = true;

    [SerializeField]
    bool Resetter;

    [SerializeField]
    GameObject Followtarget; //gameobject that camera will move towards, will follow the player if no target given.

    [SerializeField]
    int FPOverrideChildTarget = -1; //if no follow target, camera will follow player child object with this index, if less than 0, camera will follow player root object.

    [SerializeField]
    GameObject LookTarget; //gameobject camera will look towards, will look at player if no target given.

    [SerializeField]
    float AxisLerp = 0.75f; //lerp of follow position limit.

    [SerializeField]
    float LookLerp = 0.5f; // lerp of looking at object and looking at middle.

    [SerializeField]
    float FollowSpeed = 10f;

    [SerializeField]
    float RotateSpeed = 2f;

    [SerializeField]
    bool SlowMotion = false;

    [SerializeField]
    float SlowMotionTimescale = 1;

    [SerializeField]
    float SlowMotionDuration = 0; //duration of slow motion. Values below or equals 0 will count as infinite duration.
    //slow motion with infinite duration is reset if autoreset is on.
    //if slow motion duration is specified, it will not not reset with autoreset.

    void Start()
    {
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camerascript>();

        if (Followtarget == null && cam != null)
        {
            if (FPOverrideChildTarget < 0)
            {
                Followtarget = GameManager.Instance.player;
            }else if (FPOverrideChildTarget > GameManager.Instance.player.transform.childCount)
            {
                Debug.LogError("Follow target child index out of range.");
            }else
            {
                Followtarget = GameManager.Instance.player.transform.GetChild(FPOverrideChildTarget).gameObject;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (Resetter && other.tag == "Player")
        {
            cam.SetcamPos();
            if (SlowMotionDuration > 0)
            {
                TimeManager.Instance.ResetTime(true);
            }
        } else if (other.tag == "Player")
        {
            cam.SetcamPos(Followtarget, LookTarget, AxisLerp, LookLerp, FollowSpeed, RotateSpeed);
        }
        if (!Resetter && SlowMotion && other.tag == "Player")
        {
            TimeManager.Instance.SetTime(SlowMotionTimescale, SlowMotionDuration);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (Autoreset && other.tag == "Player")
        {
            cam.SetcamPos();
        }
        if (Autoreset && other.tag == "Player" && SlowMotion && SlowMotionDuration <= 0)
        {
            TimeManager.Instance.ResetTime(true);
        }
    }
}
