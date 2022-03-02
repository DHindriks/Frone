using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackManager : MonoBehaviour {

    [SerializeField]
    GameObject QTEObj;

    [SerializeField]
    Camerascript Cam;

    bool SequenceActive;

    int AmountLeft;
    bool Autoreset;
    GameObject LatestQTE;
    GameObject OldQTE;
    GameObject playerobj;
    GameObject Hackedobj;
    Player playerscript;
    Vector3 playervelocity;
    private void Start()
    {
        playerobj = GameManager.Instance.player;
        playerscript = playerobj.GetComponent<Player>();
    }

    public void StartSequence(int amount, GameObject HackedController, GameObject Console, bool AutoReset)
    {
        if (SequenceActive == true)
        {
            Debug.LogError("tried to start another sequence while current is active, this shouldn't happen.");
        }else
        {
            Cam.SetcamPos(Console, Console, 0, 0, 2, 0.1f);
            Autoreset = AutoReset;
            AmountLeft = amount;
            Hackedobj = HackedController;
            SequenceActive = true;
            playervelocity = playerobj.GetComponent<Rigidbody>().velocity;
            playerscript.Invincible = true;
            playerscript.Allowcontrols = false;
            playerobj.GetComponent<Rigidbody>().isKinematic = true;
            TimeManager.Instance.SetTime(0.5f, 0);
            SpawnNext();
        }
    }

    public void SpawnNext()
    {
        if (AmountLeft > 0)
        {
            if(LatestQTE != null)
            {
                OldQTE = LatestQTE;
            }
            LatestQTE = Instantiate(QTEObj, transform);
            LatestQTE.GetComponent<HackQTE>().manager = GetComponent<HackManager>(); 
            //TODO: getcomponent for every new QTE? Can the manager be a singleton?
            if (OldQTE != null)
            {
                OldQTE.transform.SetParent(LatestQTE.transform);
            }
            AmountLeft--;
        }else
        {
            CloseQTE();
            Win();
            if (Autoreset == true)
            {
                ResetPlayer();
            }

        }
    }

    void CloseQTE()
    {
        SequenceActive = false;
        TimeManager.Instance.ResetTime();
        StartCoroutine("ShrinkObject", LatestQTE);
    }

    public void ResetPlayer()
    {
        playerobj.GetComponent<Rigidbody>().isKinematic = false;
        playerobj.GetComponent<Rigidbody>().velocity = playervelocity;
        playerscript.Allowcontrols = true;
        playerscript.Invincible = false;
        Cam.SetcamPos();
    }

    public void Lose()
    {
        CloseQTE();
        ResetPlayer();
    }

    void Win()
    {
        Hackedobj.SetActive(!Hackedobj.activeSelf);
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
