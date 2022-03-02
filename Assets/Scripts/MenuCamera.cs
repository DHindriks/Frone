using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Positions
{
    [Header("Drag position object here")]
    public GameObject Positionobject;
}


public class MenuCamera : MonoBehaviour {


    [SerializeField]
    GameObject target;
    [SerializeField]
    List<Positions> positions;
    int currentindex;
    GameObject CurrentTarget;

    // Use this for initialization
    void Start () {
        InvokeRepeating("ChangePos", 0, 5);
	}

    private void Update()
    {
        this.transform.root.position = Vector3.Lerp(transform.position, target.transform.position, 1f );
        this.gameObject.transform.LookAt(target.transform);

        if (CurrentTarget != null)
        {
            this.transform.position = Vector3.Lerp(transform.position, CurrentTarget.transform.position, 0.05f);
        }
    }


    void ChangePos ()
    {
        currentindex += 1;
            if (currentindex >= positions.Count)
        {

            currentindex = 0;
            CurrentTarget = positions[currentindex].Positionobject;

        } else
        {
            CurrentTarget = positions[currentindex].Positionobject;
        }
    }
}
