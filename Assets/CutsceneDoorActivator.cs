using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneDoorActivator : MonoBehaviour
{
    [SerializeField]
    Movedoor movedoor;

    private void OnDisable()
    {
        movedoor.SetAnim(false);
    }

    private void OnEnable()
    {
        movedoor.SetAnim(true);
    }
}
