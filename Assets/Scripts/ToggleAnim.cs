using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleAnim : MonoBehaviour {

    [SerializeField]
    Animator animator;

    [SerializeField]
    bool SetTo;

    void Awake()
    {
        animator.SetBool("Enabled", SetTo);
        GameObject.FindWithTag("UIManager").GetComponent<HackManager>().ResetPlayer();
    }
}
