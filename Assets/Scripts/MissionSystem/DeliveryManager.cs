using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MissionManager {
    [SerializeField]
    GameObject RopePrefab;

    public GameObject Rope;

    protected override void Awake()
    {
        base.Awake();
        //Invoke("SetUp", 0.2f);
        SetUp();
        DialogueManager.Instance.AddDialogue("Package attached.", Characters.Zero);
        DialogueManager.Instance.AddDialogue("Good luck.", Characters.Zero);
    }

    void SetUp()
    {
        GameManager.Instance.ObjectiveManager.SetText("Deliver package (0%)");
        Rope = Instantiate(RopePrefab);
        Rope.transform.position = GameManager.Instance.player.transform.position;
        Rope.transform.GetChild(0).GetComponent<ConfigurableJoint>().connectedBody = GameManager.Instance.player.GetComponent<Rigidbody>();
        ObstacleEvent DropOff = new ObstacleEvent
        {
            ObstacleID = 1,
            TileCount = Mathf.FloorToInt(base.Length * 0.75f)
        };
        base.WorldGen.obstacleEvents.Add(DropOff);

    }

}
