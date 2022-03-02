using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityBase : MonoBehaviour {

    [SerializeField]
    public Sprite icon;
    [HideInInspector]
    public Player player;
    //[HideInInspector]
    public bool LockAbility = true;
    // Use this for initialization
	protected virtual void Start () {
        if (transform.root.gameObject.GetComponent<Player>() != null)
        {
            player = transform.root.gameObject.GetComponent<Player>();
            ResetUI();
        }
    }

    public virtual void ResetUI()
    {
        if (player.AbilityUI != null)
        {
            player.AbilityUI.transform.GetChild(0).GetComponent<Image>().color = GameManager.Instance.CurrentEnergy;
            player.AbilityUI.GetComponent<Image>().sprite = icon;
        }
    }
}
