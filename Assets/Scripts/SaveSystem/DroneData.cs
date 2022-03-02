using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DroneData {
    public string Name;
    public int Unlocked;
    public string PrimaryColor;
    public string SecondaryColor;
    public string DetailColor;
    public string EnergyColor;

    public DroneData (DroneSettings droneSettings)
    {
        Name = droneSettings.gameObject.name;
        Unlocked = droneSettings.UnlockState;
        PrimaryColor = ColorUtility.ToHtmlStringRGBA(droneSettings.PrimaryColor);
        SecondaryColor = ColorUtility.ToHtmlStringRGBA(droneSettings.SecondaryColor);
        DetailColor = ColorUtility.ToHtmlStringRGBA(droneSettings.DetailsColor);
        EnergyColor = ColorUtility.ToHtmlStringRGBA(droneSettings.EnergyColor);
    }
}


