using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractionManager : MissionManager {

    bool ObjectiveReachEnd;

    void Start()
    {
        //Invoke("SetUp", 0.2f);
        SetUp();
    }

    void SetUp()
    {
        MissionEvents.ReachEnd.AddListener(CompleteReachEnd);
        GameManager.Instance.ObjectiveManager.SetText("Reach extraction point (0%)");
        ResetList();
    }

    void CompleteReachEnd()
    {
        ObjectiveReachEnd = true;
        GameManager.Instance.ObjectiveManager.SetText("Reach extraction point (100%)");
        ResetList();
    }

    void ResetList()
    {
        Objectives.Clear();
        Objectives.Add(ObjectiveReachEnd);
        base.CheckObjectives(Objectives);
    }

}
