using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiscordPresence;

public class MissionManager : MonoBehaviour {

    [HideInInspector]
    public Spawnmanager WorldGen;
    [HideInInspector]
    public int Length;

    //this list is used to store all objective bools, mission willbe set to complete if all bools are true
    [HideInInspector]
    protected List<bool> Objectives = new List<bool>();

    // Use this for initialization
    protected virtual void Awake () {
        WorldGen = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<Spawnmanager>();
        
        //set difficulty and area
        WorldGen.SwitchArea = false;
        for (int i = 0; i <= WorldGen.areas.Count; i++)
        {
            if (WorldGen.areas[i].AreaName == MissionController.Instance.CurrentMission.MissionArea.Name)
            {
                WorldGen.SetNextArea(i, WorldGen.areas);
                Debug.Log("Area set to: " + MissionController.Instance.CurrentMission.MissionArea.Name);
                break;
            }
        }
        PresenceManager.UpdatePresence(detail: MissionController.Instance.CurrentMission.MissionType.MissionType.ToString());

        Length = 15 + Mathf.RoundToInt(MissionController.Instance.CurrentMission.Difficulty * 10) + Random.Range(-8, 11);
        WorldGen.CurrentArea.EasyChance = MissionController.Instance.CurveEasy.Evaluate(MissionController.Instance.CurrentMission.Difficulty);
        WorldGen.CurrentArea.HardChance = MissionController.Instance.CurveHard.Evaluate(MissionController.Instance.CurrentMission.Difficulty);
        WorldGen.CurrentArea.MediumChance = 1f - (WorldGen.CurrentArea.EasyChance + WorldGen.CurrentArea.HardChance);

        //set Default events(spawn level end)
        ObstacleEvent SpawnDropship = new ObstacleEvent
        {
            ObstacleID = 0,
            TileCount = Length,
        };
        WorldGen.obstacleEvents.Add(SpawnDropship);
    }

    protected virtual void CheckObjectives(List<bool> ListToRead)
    {
        foreach (bool Objective in ListToRead)
        {
            if (!Objective)
            {
                Debug.Log("Objective False, returning");
                return;
            }
        }
        //mission complete, player will recieve full mission reward on mission end(even if player dies) 
        Debug.Log("Mission completed");
    }
}

public class Objective
{
    bool Completed;
    string ObjectiveText;
}


