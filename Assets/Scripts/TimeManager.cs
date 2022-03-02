using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

    public static TimeManager Instance = null;

    bool SlowActive = false;

    float LastDuration = 0;

    float LastStartTime = 0;

    float LerpTime = 5;

    Coroutine TimeLerp;

	// Use this for initialization
	void Awake () {
		if (Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
	
    public void SetTime(float timeScale, float Duration = 0)
    {
        if(SlowActive == true)
        {
            if(timeScale < Time.timeScale)
            {
                if (TimeLerp != null)
                {
                    StopCoroutine(TimeLerp);
                }
                TimeLerp = StartCoroutine(ChangeTime(timeScale, LerpTime));
            }
            if (Duration <= 0)
            {
                CancelInvoke();
            }else if (Duration > LastDuration - LastStartTime)
            {
                CancelInvoke();
                Invoke("InvokedReset", Duration);
            }
        }
        else
        {
            CancelInvoke();
            if (TimeLerp != null)
            {
                StopCoroutine(TimeLerp);
            }
            TimeLerp = StartCoroutine(ChangeTime(timeScale, LerpTime));
            if (Duration > 0)
            {
                Invoke("InvokedReset", Duration);
            }
        }
        SlowActive = true;
    }

    void InvokedReset()
    {
        ResetTime();
    }

    public void ResetTime(bool QuickLerp = false)
    {
        if (!QuickLerp)
        {
            if (TimeLerp != null)
            {
                StopCoroutine(TimeLerp);
            }
            TimeLerp = StartCoroutine(ChangeTime(1, LerpTime));
        }else
        {
            if (TimeLerp != null)
            {
                StopCoroutine(TimeLerp);
            }
            TimeLerp = StartCoroutine(ChangeTime(1, 1));
        }

        SlowActive = false;
    }

    //TODO: Use this to lerp time change
    IEnumerator ChangeTime(float TimeScale, float LerpTime)
    {
        StopCoroutine(ChangeTime(0, 0));
        float Rate = 1.0f / LerpTime;
        float i = 0;

        while (i < 1)
        {
            i += Time.deltaTime * Rate;
            Time.timeScale = Mathf.Lerp(Time.timeScale, TimeScale, i);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return 0;
        }

        Time.timeScale = TimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

}
