using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour {
    public float Score;
    bool DisplayScore; //if true: displays score in bottom UI, else displays current objective
    bool Countscore; //Score is not counted when false.
    float Distance;
    float olddistance;
    float Multiplier;
    [SerializeField]
    Player player;

    [SerializeField]
    Text Scorecounter;

    //TODO: stop using this.transform.position, use vector3 set by player when leaving dropship, what????
    // Update is called once per frame
    void Awake()
    {
        SetUp();
        DisplayScore = true;
    }

    void SetUp()
    {
        GameManager.Instance.ObjectiveManager = this;
    }

    public void SetUI(bool enabled)
    {
        if (enabled)
        {
            transform.position = player.transform.position;
            Distance = 0;
            olddistance = 0;
            Countscore = true;
        }else
        {
            Countscore = false;
        }
        Scorecounter.transform.parent.gameObject.SetActive(enabled);   
    }

    public void SetText(string text = null)
    {
        if (text != null)
        {
            DisplayScore = false;
            Scorecounter.text = text;
        }else
        {
            DisplayScore = true;
        }
    }

    void DispScore()
    {
        Scorecounter.text = Mathf.Floor(Score).ToString();
    }

    void CountScore()
    {
        Score += (Distance - olddistance) * Multiplier;
    }

	void Update () {
        if (DisplayScore)
        {
            DispScore();
        }
        Multiplier = player.ScoreMultiplier;
        player.speed = player.Startingspeed + Distance / player.Acceleration;
        Distance = Vector3.Distance(this.transform.position, player.gameObject.transform.position);
        if (Countscore)
        {
            CountScore();
        }
        olddistance = Distance;
	}
}


