using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorpickerUI : MonoBehaviour {
    [HideInInspector]
    public GameObject ActivePicker;

    public void ResetColors()
    {
        foreach (ColorManager manager in GetComponentsInChildren<ColorManager>())
        {
            manager.ResetColors();
        }
    }

    public void ClosePrevious()
    {
        if (ActivePicker != null)
        {
            ActivePicker.SetActive(false);
        }

    }



}
