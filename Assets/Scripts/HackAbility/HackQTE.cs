using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackQTE : MonoBehaviour {

    [SerializeField]
    List<GameObject> PlayerObjs;

    [SerializeField]
    GameObject TimerPointer;

    [SerializeField]
    List<GameObject> Choices;

    GameObject CorrectChoice;
    int SelectedChoice;
    GameObject SelectedObject;

    [SerializeField]
    float TimeLimit;

    float Activetime = 0;
    bool Completed = false;

    Color Playercolor;

    public HackManager manager;

    void Start()
    {
        SelectedChoice = Choices.Count / 2;
        SelectedObject = Choices[SelectedChoice];
        Playercolor = GameManager.Instance.CurrentEnergy;
        foreach (GameObject gameObject in PlayerObjs)
        {
            gameObject.GetComponent<Image>().color = Playercolor;
        }
        pickchoice();
    }

    void pickchoice()
    {
        foreach(GameObject gameObject in Choices)
        {
            gameObject.GetComponent<RawImage>().color = Color.red;
        }
        int GeneratedChoice = Random.Range(0, Choices.Count);
        CorrectChoice = Choices[GeneratedChoice];
        CorrectChoice.GetComponent<RawImage>().color = Color.green;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && !Completed)
        {
            if (SelectedChoice - 1 <= 0)
            {
                SelectedChoice = 0;
            }else
            {
                SelectedChoice -= 1;
            }
            SelectedObject = Choices[SelectedChoice];
            Vector3 relative = transform.InverseTransformPoint(Choices[SelectedChoice].transform.position);
            float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
            TimerPointer.transform.rotation = Quaternion.Euler(0, 0, -angle);
        }

        if (Input.GetKeyDown(KeyCode.D) && !Completed)
        {
            if (SelectedChoice + 1 >= Choices.Count)
            {
                SelectedChoice = Choices.Count - 1;
            }else
            {
                SelectedChoice += 1;
            }
            SelectedObject = Choices[SelectedChoice];
            Vector3 relative = transform.InverseTransformPoint(Choices[SelectedChoice].transform.position);
            float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
            TimerPointer.transform.rotation = Quaternion.Euler(0, 0, -angle);
        }
        if (Completed == false)
        {
            Activetime += Time.deltaTime;
            TimerPointer.transform.GetChild(0).localScale = new Vector3(0.8f, Activetime / TimeLimit, 1);
            //this should work when ui sprites are added, avoids sprite stretching
            //TimerPointer.transform.GetChild(0).GetComponent<Image>().fillAmount = Mathf.Lerp(0, 1, Activetime / TimeLimit);
            if (Activetime >= TimeLimit)
            {
                Completed = true;
                if(SelectedObject == CorrectChoice)
                {
                    CorrectChoice.GetComponent<RawImage>().color = Playercolor;
                    StartCoroutine(MoveUI(transform.position - CorrectChoice.transform.parent.localPosition, 8));
                }else
                {
                    manager.Lose();
                }
            }
        }

    }



    IEnumerator MoveUI(Vector3 end, float speed)
    {
        //while you are far enough away to move
        while (Vector3.Distance(this.transform.position, end) > speed)
        {
            //MoveTowards the end position by a given distance
            this.transform.position = Vector3.MoveTowards(this.transform.position, end, speed);
            //wait for a frame and repeat
            yield return 0;
        }
        //Since you are really really close, now you can just go to the final position.
        this.transform.position = end;
        manager.SpawnNext();
        CorrectChoice.transform.parent.gameObject.SetActive(false);
        Destroy(this);
    }
}
