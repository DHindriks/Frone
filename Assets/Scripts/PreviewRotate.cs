using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewRotate : MonoBehaviour {

    public bool AllowRotation;
    float speed = 500f;

    Vector3 rotation;

    void Update()
    {
        if (Input.GetMouseButton(0) && AllowRotation == true)
        {
            rotation = transform.rotation.eulerAngles;
            rotation.y = -Input.GetAxis("Mouse X") * Time.deltaTime * speed;
            this.GetComponent<Rigidbody>().AddTorque(rotation);
        }
    }
}
