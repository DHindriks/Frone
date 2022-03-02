using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthObj : MonoBehaviour {
    float Health;

    void ApplyDamage(float amount)
    {
        Health -= amount;
    }
}
