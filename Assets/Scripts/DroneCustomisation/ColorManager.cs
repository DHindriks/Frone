using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Colortypes
{
    Primary,
    Secondary,
    Details,
    Energy
}

public class ColorManager : MonoBehaviour {

    [HideInInspector]
    public CUIColorPicker picker;

    [HideInInspector]
    public Image SelectedColorPreview;

    public DroneSettings Colorsettings;

    [SerializeField]
    Colortypes colortype;

    public Color Mat; //color which will be changed

    [SerializeField]
    GameObject PickerPrefab; //prefab will be spawned if it isn't already.
    GameObject ColorPickerobj;
    ColorpickerUI UIManager;
    [SerializeField]
    PreviewRotate RotationManager;
	// Use this for initialization
	void Awake () {
        UIManager = this.transform.parent.GetComponent<ColorpickerUI>();
        if (RotationManager == null)
        RotationManager = GameObject.FindWithTag("Player").GetComponent<PreviewRotate>();
        SelectedColorPreview = transform.GetChild(0).GetComponent<Image>();
        //ResetColors();
	}

    public void ResetColors()
    {
        Colorsettings = RotationManager.GetComponentInChildren<DroneSettings>();
        switch (colortype) //sets correct start color each time player interacts with color picker
        {
            case Colortypes.Primary:
                Mat = Colorsettings.PrimaryColor;
                SelectedColorPreview.color = Colorsettings.PrimaryColor;
                break;
            case Colortypes.Secondary:
                Mat = Colorsettings.SecondaryColor;
                SelectedColorPreview.color = Colorsettings.SecondaryColor;
                break;
            case Colortypes.Details:
                Mat = Colorsettings.DetailsColor;
                SelectedColorPreview.color = Colorsettings.DetailsColor;
                break;
            case Colortypes.Energy:
                Mat = Colorsettings.EnergyColor;
                SelectedColorPreview.color = Colorsettings.EnergyColor;
                break;
        }
    }

    public void ToggleColorPicker ()
    {

        Colorsettings = RotationManager.GetComponentInChildren<DroneSettings>();
        if (ColorPickerobj == null)
        {
            ColorPickerobj = Instantiate(PickerPrefab, transform.parent.parent) as GameObject;
            //ColorPickerobj.transform.localScale = Vector3.one;
            ColorPickerobj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            ColorPickerobj.transform.position += ColorPickerobj.transform.right;
            picker = ColorPickerobj.GetComponentInChildren<CUIColorPicker>();
            picker.Manager = this;
        } else
        {
            ColorPickerobj.SetActive(!ColorPickerobj.activeSelf);
        }
        if (ColorPickerobj != UIManager.ActivePicker)
        {
            UIManager.ClosePrevious();
        }
        UIManager.ActivePicker = ColorPickerobj;

        if (UIManager.ActivePicker.activeSelf == false)
        {
            RotationManager.AllowRotation = true;
            SaveSystem.SaveDrone(Colorsettings);
        } else
        {
            RotationManager.AllowRotation = false;
        }

        picker.CurrentSelected = Colorsettings;
        picker.playerColor = colortype;

        switch (colortype) //sets correct start color each time player interacts with color picker
        {
            case Colortypes.Primary:
                picker.Color = Colorsettings.PrimaryColor;
                Mat = Colorsettings.PrimaryColor;
                break;
            case Colortypes.Secondary:
                picker.Color = Colorsettings.SecondaryColor;
                Mat = Colorsettings.SecondaryColor;
                break;
            case Colortypes.Details:
                picker.Color = Colorsettings.DetailsColor;
                Mat = Colorsettings.DetailsColor;
                break;
            case Colortypes.Energy:
                picker.Color = Colorsettings.EnergyColor;
                Mat = Colorsettings.EnergyColor;
                break;
        }
    }


}
