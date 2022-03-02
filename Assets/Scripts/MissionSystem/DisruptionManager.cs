using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisruptionManager : MissionManager {

    bool ObjectiveReachEnd;

    void Start()
    {
        //Invoke("SetUp", 0.2f);
        SetUp();
    }

    void SetUp()
    {
        ObstacleEvent SpawnDropship = new ObstacleEvent
        {
            ObstacleID = 2,
            TileCount = Length/2,
        };
        WorldGen.obstacleEvents.Add(SpawnDropship);
        Debug.Log("disruption setup");
        GameManager.Instance.ObjectiveManager.SetText("Destroy alien tower (0%)");
        ResetList();
    }

    void CompleteObjective()
    {
        ObjectiveReachEnd = true;
        GameManager.Instance.ObjectiveManager.SetText("Objective complete");
        ResetList();
    }

    void ResetList()
    {
        Objectives.Clear();
        Objectives.Add(ObjectiveReachEnd);
        base.CheckObjectives(Objectives);
    }

}
