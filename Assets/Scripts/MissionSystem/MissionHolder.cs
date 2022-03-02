using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Playables;

public class MissionHolder : MonoBehaviour {

    Text ButtonText;
    Mission CurrentMission;

    public PlayableDirector Cutscene;

	// Use this for initialization
	void Awake () {
        CurrentMission = MissionController.Instance.GenerateMission();
        ButtonText = GetComponentInChildren<Text>();
        ButtonText.text = CurrentMission.MissionArea.Name + " " +  CurrentMission.MissionType.MissionType.ToString();
	}
	
    public void StartMission()
    {
        MissionController.Instance.StartMission(CurrentMission, Cutscene);
    }
}
