using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movedoor : MonoBehaviour {

    [SerializeField]
    Vector3 Moveto;

    [SerializeField]
    Transform door;

    [SerializeField]
    GameObject Activator;

    [SerializeField]
    float LerpTime;



    IEnumerator MoveDoor()
    {
        float Rate = 1.0f / LerpTime;
        float i = 0;

        while (i < 1)
        {
            i += Time.deltaTime * Rate;
            door.localPosition = Vector3.Lerp(door.localPosition, Moveto, i);
            yield return 0;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireCube(transform.TransformPoint(Moveto), door.GetComponent<BoxCollider>().size);
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireCube(transform.TransformPoint(gameObject.GetComponent<BoxCollider>().center), gameObject.GetComponent<BoxCollider>().size);
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine("MoveDoor");
        }
    }

    public void ToggleAnim (bool Toggle = false) 
    {
        Activator.SetActive(!Activator.activeSelf);
    }

    public void SetAnim(bool SetTo = false)
    {
        Animator animator = GetComponent<Animator>();
        animator.SetBool("Open", SetTo);
    }
}
