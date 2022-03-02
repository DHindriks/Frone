using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OLDObjectiveManager : MonoBehaviour {

    public GameManager GameManager;

    [SerializeField]
    Text ObjectiveTXT;

    string CurrentName;
    int MaxAmount;
    int CurrentAmount;

    public void SetObjective(string name, int amount, bool Reset = false)
    {
        if (name != CurrentName || Reset == true)
        {
            CurrentName = name;
            MaxAmount = amount;
            CurrentAmount = 0;
            ObjectiveTXT.transform.parent.gameObject.SetActive(true);
            UpdateWindow();
        }
    }

    public void AddObjective(int amount)
    {
        if (CurrentName != null)
        {
            CurrentAmount += amount;
            UpdateWindow();
        }
    }

    public void UpdateWindow ()
    {
        ObjectiveTXT.text = CurrentName + "\n" + CurrentAmount + " / " + MaxAmount;
        if (CurrentAmount >= MaxAmount)
        {
            CompleteObjective();
        }
    }

    public void RemoveObjective()
    {
        CurrentName = null;
        CurrentAmount = 0;
        ObjectiveTXT.text = null;
        ObjectiveTXT.transform.parent.gameObject.SetActive(false);
    }

    //TODO: Very old, remove this and potential references to this.
    void CompleteObjective()
    {
        if (CurrentName == "Alert Level")
        {
            GameManager.SpawnManager.SetNextArea(0, GameManager.SpawnManager.SpecialAreas);
        }
        if (CurrentName == "Destroy Energy Cores")
        {
            GameManager.SpawnManager.PickNextArea();
        }

        CurrentName = null;
        CurrentAmount = 0;
    }

}
