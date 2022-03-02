using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DiscordPresence;

[System.Serializable]
public class Areas
{
    public string AreaName;
    public bool Excluded = false;
    [Space(10)]
    [Header("Area theme color")]
    public Color color;
    [Space(10)]
    [Header("Default tile prefab")]
    public GameObject DefaultTile;
    public float tileLength;
    public List<GameObject> EasyObstaclePrefabs;
    [Space(5)]
    public List<GameObject> MediumObstaclePrefabs;
    [Space(5)]
    public List<GameObject> HardObstaclePrefabs;
    [Space(5)]
    [Header("Special Tile prefabs")]
    public List<GameObject> SpecialTiles;
    [Space(10)]
    [Header("Decoration prefabs")]
    public List<GameObject> DecorationsLeft;
    public List<GameObject> DecorationsRight;
    [Space(10)]
    [Header("Background prefabs")]
    public List<GameObject> BackgroundPropsLeft;
    public List<GameObject> BackgroundPropsRight;
    [Space(10)]
    [Header("Encounter prefabs")]
    public List<GameObject> Encounters;
    [Space(10)]
    public List<GameObject> AreaEventTiles;

    //difficulty settings (special chance is seperate, all other chances must add up to 1.0f)
    [Space(10)]
    [Header("Area obstacle appearance rates")]
    public float EasyChance;
    public float MediumChance;
    public float HardChance;
    public float SpecialChance;
    public bool UseSpecials = true;
}

[System.Serializable]
public class ObstacleEvent
{
    public int TileCount;
    public int ObstacleID = -1;
    public int LeftBID = -1;
    public int RightBID = -1;
    public int LeftSID = -1;
    public int RightSID = -1;
}

/// <summary>
/// The spawn manager generates the world, it has a lot of settings that can be influenced by other scripts that manipulate the world, like the mission managers.
/// </summary>

public class Spawnmanager : MonoBehaviour {


    [SerializeField]
    GameObject SpawnTrigger;
    [SerializeField, Range(0.0f, 100.0f)]
    float EncounterFrequency;
    [Header("Add tile prefabs")]
    public List<Areas> areas;
    [Space(10)]
    [Header("Special obstacle prefabs"), SerializeField]
    List<GameObject> SpecialObstacles;
    [Header("Special decoration prefabs"), SerializeField]
    List<GameObject> SpecialDecorations;

    [Header("Obstacle Id | Tile number")]
    public List<ObstacleEvent> obstacleEvents;

    [HideInInspector]
    public GameObject OldTile;
    [HideInInspector]
    public GameObject NewestTile;
    [HideInInspector]
    public List<Areas> SpawnableAreas = new List<Areas>();
    [HideInInspector]
    public List<Areas> SpecialAreas = new List<Areas>();
    List<Areas> CurrentList;
    public bool SwitchArea;





    int TileQueue;

    int EncounterTimer;
    //TODO: the current area was only saved as an index int?? why? added a real currentarea here, but the index var needs to go.
    int CurrentAreaIndex;
    //[HideInInspector]
    public Areas CurrentArea;

    Areas PreviousArea;
    float Extralength = 0; //used to create smooth transitions between different tile lengths
    int PassedTiles;
    int PassedAreas;
    int totalTiles;

    GameObject LastAreaTile;
    Dictionary<string, Areas> LastAreaTiles = new Dictionary<string, Areas>();
    void Start()
    {
        for (int i = 0; i < areas.Count; i++)
        {
            if (areas[i].Excluded == false)
            {
                SpawnableAreas.Add(areas[i]);
            } else
            {
                SpecialAreas.Add(areas[i]);
            }
        }
        SetReference();
        CurrentList = SpawnableAreas;
    }

    void SetReference()
    {
        if (GameManager.Instance.SpawnManager == null)
        {
             GameManager.Instance.SpawnManager = this;
        }
    }

    //manually sets next area, used to set excluded areas
    public void SetNextArea(int index, List<Areas> list)
    {
        if (NewestTile != null)
        LastAreaTile = NewestTile;
        PreviousArea = CurrentArea;
        CurrentList = list;
        CurrentAreaIndex = index;
        PassedTiles = 0;
        CurrentArea = list[index];
        if (NewestTile != null)
        LastAreaTiles.Add(NewestTile.gameObject.name, CurrentList[CurrentAreaIndex]);
        Debug.Log(NewestTile + " " + CurrentList[CurrentAreaIndex].AreaName);
        Extralength = (PreviousArea.tileLength / 2) - (CurrentArea.tileLength / 2);
        SetDecorations();

        PresenceManager.UpdatePresence(largeText: CurrentArea.AreaName);
    }
    //picks a random area
    public void PickNextArea()
    {
        if (NewestTile != null)
        LastAreaTile = NewestTile;
        PreviousArea = CurrentArea;
        CurrentList = SpawnableAreas;
        CurrentAreaIndex = Random.Range(0, SpawnableAreas.Count);
        if (NewestTile != null)
        LastAreaTiles.Add(NewestTile.gameObject.name, CurrentList[CurrentAreaIndex]);
        CurrentArea = CurrentList[CurrentAreaIndex];
        Extralength = (PreviousArea.tileLength / 2) - (CurrentArea.tileLength / 2);
        SetDecorations();
    }

    void SetDecorations()
    {
        //Automatically fill decorations & background if one list is empty
        if (CurrentArea.DecorationsLeft.Count == 0 && CurrentArea.DecorationsRight.Count > 0)
        {
            CurrentArea.DecorationsLeft = CurrentArea.DecorationsRight;
        }
        else if (CurrentArea.DecorationsRight.Count == 0 && CurrentArea.DecorationsLeft.Count > 0)
        {
            CurrentArea.DecorationsRight = CurrentArea.DecorationsLeft;
        }

        if (CurrentArea.BackgroundPropsLeft.Count == 0 && CurrentArea.BackgroundPropsRight.Count > 0)
        {
            CurrentArea.BackgroundPropsLeft = CurrentArea.BackgroundPropsRight;
        }
        else if (CurrentArea.BackgroundPropsRight.Count == 0 && CurrentArea.BackgroundPropsLeft.Count > 0)
        {
            CurrentArea.BackgroundPropsRight = CurrentArea.BackgroundPropsLeft;
        }
    }

    //spawns a specified tile
    public void SetTile(int AreaIndex, int TileIndex, List<Areas> list)
    {
        GameObject spawnedTile = Instantiate(list[AreaIndex].SpecialTiles[TileIndex], NewestTile.transform.position + new Vector3(0, -40, CurrentArea.tileLength), Quaternion.identity) as GameObject;
        StartCoroutine("MoveTile", spawnedTile);
        NewestTile = spawnedTile;
        TileQueue++;
    }

    //generates random number between 0 and list count
    int PickRandom(List<GameObject> list)
    {
        return Random.Range(0, list.Count);
    }

    IEnumerator MoveTile(GameObject Tile)
    {
        while (Tile.transform.position.y < 0)
        {
            Tile.transform.position = Vector3.Lerp(Tile.transform.position, new Vector3(Tile.transform.position.x, 1, Tile.transform.position.z), 4 * Time.deltaTime);
            yield return null;
        }
        Tile.transform.position = new Vector3(Tile.transform.position.x, 0, Tile.transform.position.z);
    }

    //spawnNewtile without argument for invokes.
    void InvokeSpawnTile()
    {
        SpawnNewTile();
    }

    //spawns a new tile with decoration objects
    public void SpawnNewTile(int EventTileIndex = -1, int SpecialDecoBIndexR = -1, int SpecialDecoSIndexR = -1, int SpecialDecoBIndexL = -1, int SpecialDecoSIndexL = -1)
    {
        if (TileQueue <= 0)
        {
            int RandomIndex;
            GameObject spawnedTile;
            //if this tile is the first of the level, spawns multiple tiles
            if (NewestTile == null)
            {
                Debug.Log("Newest tile is null, current area is: " + CurrentArea.AreaName);
                if (CurrentArea.AreaName == "")
                {
                    Debug.Log("Area generated");
                    CurrentList = SpawnableAreas;
                    CurrentAreaIndex = Random.Range(0, SpawnableAreas.Count);
                    CurrentArea = CurrentList[CurrentAreaIndex];
                    PreviousArea = CurrentList[CurrentAreaIndex];
                }
                Debug.Log("Newest tile is null, current area is: " + CurrentArea.AreaName);

                    SetDecorations();
                GameManager.Instance.FloorManager.GetComponent<ManageFloor>().FloorColor = CurrentArea.color;
                RandomIndex = Random.Range(0, CurrentArea.SpecialTiles.Count);
                spawnedTile = Instantiate(CurrentArea.DefaultTile, new Vector3(0, 0, 0) + new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                totalTiles = 0;
                spawnedTile.name += totalTiles;
                NewestTile = spawnedTile;
                for (int i = 0; i <= 4; i++)
                {
                    Invoke("InvokeSpawnTile", (i + 1) / 10);
                }
            }
            else //normal single tile spawning
            {
                //special tile
                if (CurrentArea.UseSpecials && Random.value <= 0.2f && EventTileIndex == -1 && CurrentArea.SpecialTiles.Count > 0)
                {
                    RandomIndex = Random.Range(0, CurrentArea.SpecialTiles.Count);
                    spawnedTile = Instantiate(CurrentArea.SpecialTiles[RandomIndex], NewestTile.transform.position + new Vector3(0, -40, CurrentArea.tileLength + Extralength), Quaternion.identity) as GameObject;
                    spawnedTile.name += totalTiles;
                    StartCoroutine("MoveTile", spawnedTile);
                    NewestTile = spawnedTile;
                }
                else //normal tile
                {
                    float DifficultyIndex = Random.value;
                    spawnedTile = Instantiate(CurrentArea.DefaultTile, NewestTile.transform.position + new Vector3(0, -40, CurrentArea.tileLength + Extralength), Quaternion.identity) as GameObject;
                    GameObject obstacle;
                    if (EventTileIndex != -1 && SpecialObstacles[EventTileIndex] != null)
                    {
                        obstacle = Instantiate(SpecialObstacles[EventTileIndex]);
                        obstacle.transform.SetParent(spawnedTile.transform);
                        obstacle.transform.position = spawnedTile.transform.position;
                        TileQueue++;
                    }
                    else if (DifficultyIndex <= CurrentArea.EasyChance && CurrentArea.EasyObstaclePrefabs.Count > 0)
                    {
                        obstacle = Instantiate(CurrentArea.EasyObstaclePrefabs[PickRandom(CurrentArea.EasyObstaclePrefabs)]);
                        obstacle.transform.SetParent(spawnedTile.transform);
                        obstacle.transform.position = spawnedTile.transform.position;
                    }else if (DifficultyIndex <= CurrentArea.MediumChance + CurrentArea.EasyChance && CurrentArea.MediumObstaclePrefabs.Count > 0)
                    {
                        obstacle = Instantiate(CurrentArea.MediumObstaclePrefabs[PickRandom(CurrentArea.MediumObstaclePrefabs)]);
                        obstacle.transform.SetParent(spawnedTile.transform);
                        obstacle.transform.position = spawnedTile.transform.position;
                    }else if (DifficultyIndex <= CurrentArea.HardChance + CurrentArea.MediumChance + CurrentArea.EasyChance && CurrentArea.HardObstaclePrefabs.Count > 0)
                    {
                        obstacle = Instantiate(CurrentArea.HardObstaclePrefabs[PickRandom(CurrentArea.HardObstaclePrefabs)]);
                        obstacle.transform.SetParent(spawnedTile.transform);
                        obstacle.transform.position = spawnedTile.transform.position;
                    }
                    else
                    {
                        Debug.LogError("obstacle difficulty could not be calculated, Combined difficulty chances don't add up to 1.0 or certain difficulties don't have obstacles assigned.");
                    }

                    spawnedTile.name += totalTiles;
                    StartCoroutine("MoveTile", spawnedTile);
                    NewestTile = spawnedTile;

                }
            }
            //spawn collider which spawns next tile on trigger enter
            GameObject triggerobj = Instantiate(SpawnTrigger);
            triggerobj.transform.SetParent(spawnedTile.transform);
            triggerobj.transform.position = spawnedTile.transform.position;


            int DecoID;
            //decoration objects
            if (CurrentArea.DecorationsRight.Count != 0 || SpecialDecoSIndexR != -1)
            {
                //right side
                if (SpecialDecoSIndexR != -1 && SpecialDecorations.Count >= SpecialDecoSIndexR) {
                    DecoID = SpecialDecoSIndexR;
                }else
                {
                    DecoID = PickRandom(CurrentArea.DecorationsRight);
                }
                GameObject DecorationRight = Instantiate(CurrentArea.DecorationsRight[DecoID], spawnedTile.transform.position + new Vector3(15, 0, 0), Quaternion.identity, spawnedTile.transform) as GameObject;
                DecorationRight.transform.Rotate(0, 0, 0);
            }

            if (CurrentArea.DecorationsLeft.Count != 0 || SpecialDecoSIndexL != -1)
            {
                //left side
                if (SpecialDecoSIndexL != -1 && SpecialDecorations.Count >= SpecialDecoSIndexL)
                {
                    DecoID = SpecialDecoSIndexL;
                }else
                {
                    DecoID = PickRandom(CurrentArea.DecorationsLeft);
                }
                GameObject DecorationLeft = Instantiate(CurrentArea.DecorationsLeft[DecoID], spawnedTile.transform.position + new Vector3(-15, 0, 0), Quaternion.identity, spawnedTile.transform) as GameObject;
                DecorationLeft.transform.Rotate(0, 180, 0);
            }

            //background objects
            if (CurrentArea.BackgroundPropsRight.Count != 0 || SpecialDecoBIndexR != -1)
            {
                //right side
                if (SpecialDecoSIndexR != -1 && SpecialDecorations.Count >= SpecialDecoBIndexR)
                {
                    DecoID = SpecialDecoBIndexR;
                }
                else
                {
                    DecoID = PickRandom(CurrentArea.BackgroundPropsRight);
                }
                GameObject BackgroundRight = Instantiate(CurrentArea.BackgroundPropsRight[DecoID], spawnedTile.transform.position + new Vector3(35, 0, 0), Quaternion.identity, spawnedTile.transform) as GameObject;
                BackgroundRight.transform.Rotate(0, 0, 0);
            }

            if (CurrentArea.BackgroundPropsLeft.Count != 0 || SpecialDecoBIndexL != -1)
            {
                //left side
                if (SpecialDecoSIndexL != -1 && SpecialDecorations.Count >= SpecialDecoBIndexL)
                {
                    DecoID = SpecialDecoBIndexL;
                }
                else
                {
                    DecoID = PickRandom(CurrentArea.BackgroundPropsLeft);
                }
                GameObject BackgroundLeft = Instantiate(CurrentArea.BackgroundPropsLeft[DecoID], spawnedTile.transform.position + new Vector3(-35, 0, 0), Quaternion.identity, spawnedTile.transform) as GameObject;
                BackgroundLeft.transform.Rotate(0, 180, 0);
            }
        }
        else
        {
            TileQueue--;
        }
        Extralength = 0;
        PassedTiles++;
        totalTiles++;

        if (Random.Range(0, 100) < EncounterFrequency && CurrentArea.Encounters.Count != 0)
        {
            GameObject encounter = Instantiate(CurrentArea.Encounters[Random.Range(0, CurrentArea.Encounters.Count)]);
            encounter.transform.position = NewestTile.transform.position;
            encounter.transform.SetParent(NewestTile.transform);
        }

        if (OldTile != null) {

            if (LastAreaTiles.ContainsKey(OldTile.gameObject.name))
            {
                Areas output;
                LastAreaTiles.TryGetValue(OldTile.gameObject.name, out output);
                
                Debug.Log("Entering: " + output.AreaName);
                GameManager.Instance.FloorManager.GetComponent<ManageFloor>().FloorColor = output.color; 
                LastAreaTiles.Remove(OldTile.gameObject.name);
            }

            Destroy(OldTile, 12);
            //Debug.Log("Last area tile:" + LastAreaTile.name + " Old tile: " + OldTile.name);
        }

        foreach(ObstacleEvent Event in obstacleEvents)
        {
            if (Event.TileCount == totalTiles)
            {
                SpawnNewTile(Event.ObstacleID, Event.RightBID, Event.RightSID, Event.LeftBID, Event.LeftSID);
            }
        }

        if (PassedTiles >= Mathf.CeilToInt(Random.Range(800, 1800) / CurrentArea.tileLength) && SwitchArea)
        {
            PickNextArea();
            PassedTiles = 0;
        }




    }
}
