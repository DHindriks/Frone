using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public static class MissionEvents
{
    public static UnityEvent ReachEnd = new UnityEvent();
}

//all missions availaible in the game
public enum MissionTypes
{
    Delivery,
    Extraction,
    Disruption,
    Patrol,
    NestRaid
}

//serializable missiontype and its corresponding manager
[System.Serializable]
public struct MissionSetup
{
    public MissionTypes MissionType;
    public MissionManager Manager;
}
//area in which missions can be played, with a list of available mission types.
[System.Serializable]
public class Area
{
    public string Name;
    [SerializeField]
    bool InRandomPool;
    [SerializeField]
    public List<MissionSetup> MissionTypePool;
}

//a mission with all data needed to be played.
public struct Mission
{
    public Area MissionArea;
    public MissionSetup MissionType;
    public float Difficulty;


}

public class MissionController : MonoBehaviour {

    public static MissionController Instance = null;

    //difficulty appearance rates
    [SerializeField]
    public AnimationCurve CurveEasy;
    [SerializeField]
    AnimationCurve CurveMedium;
    [SerializeField]
    public AnimationCurve CurveHard;

    //list of areas to pick levels from(potential event areas will be excluded from this list)
    [SerializeField]
    List<Area> AreaList;
    bool StartingMission = false;
    public Mission CurrentMission;
    PlayableDirector Cutscene;
    [HideInInspector]
    public GameObject CurrentMissionManager;

    public Mission GenerateMission()
    {
        Mission GeneratedMission = new Mission();
        GeneratedMission.MissionArea = AreaList[Random.Range(0, AreaList.Count)];
        GeneratedMission.MissionType = GeneratedMission.MissionArea.MissionTypePool[Random.Range(0, GeneratedMission.MissionArea.MissionTypePool.Count)];
        GeneratedMission.Difficulty = Random.Range(0f, 1f);
        return GeneratedMission;
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (Instance == null)
        {
            Instance = this;
        }
        SceneManager.activeSceneChanged += SceneChanged;
    }

    public void StartCutscene()
    {
        Cutscene.Play();
    }

    public void StartMission(Mission MissionToLoad, PlayableDirector cut = null)
    {
        StartingMission = true;
        CurrentMission = MissionToLoad;
        if (cut != null)
        {
            Cutscene = cut;
            StartCutscene();
            Invoke("ChangeScene", (float)Cutscene.playableAsset.duration);
            Debug.Log((float)Cutscene.playableAsset.duration);
        }else
        {
            ChangeScene();
        }
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("Ingame");
    }

    void SceneChanged(Scene current, Scene next)
    {
        CurrentMissionManager = null;
        if (SceneManager.GetActiveScene().name == "Ingame" && StartingMission)
        {
            CurrentMissionManager = Instantiate(CurrentMission.MissionType.Manager.gameObject);
        }
        StartingMission = false;
    }
    

    

}
