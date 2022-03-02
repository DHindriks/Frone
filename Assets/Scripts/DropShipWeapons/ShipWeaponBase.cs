using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ShipWeaponBase : MonoBehaviour {

    public DropshipAttack AttackBase;
    public Sprite Icon;

	// Use this for initialization
	protected virtual void Start () {
        AttackBase = transform.parent.parent.GetComponent<DropshipAttack>();

        AttackBase.player.AbilityUI.GetComponent<Image>().sprite = Icon;
    }

    protected virtual void OnDestroy()
    {
        AttackBase.player.GetComponentInChildren<AbilityBase>().ResetUI();
        GameManager.Instance.cam.SetcamPos();
        GameManager.Instance.cam.ChangeZoom();
    }
}
