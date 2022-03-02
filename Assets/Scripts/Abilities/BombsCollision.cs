using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombsCollision : MonoBehaviour {

    public List<ParticleCollisionEvent> collisionEvents;

    public float Radius, ExpStrength;

    ParticleSystem bombs;

    void Start()
    {
        bombs = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Obstacle" || other.tag == "Danger")
        {
            Destroy(other);
        }

            int numCollisionEvents = bombs.GetCollisionEvents(other, collisionEvents);
            if (numCollisionEvents > 0)
            {
                Vector3 pos = collisionEvents[0].intersection;
                Collider[] colliders = Physics.OverlapSphere(pos, Radius);
                foreach (Collider coll in colliders)
                {
                    Rigidbody rb = coll.GetComponent<Rigidbody>();
                    if (rb && coll.tag == "Obstacle" || rb && coll.tag == "Danger")
                    {
                        rb.transform.SetParent(rb.transform.root);
                        rb.isKinematic = false;
                        rb.AddExplosionForce(ExpStrength, pos, Radius);
                    }
                }
            }
    }
}
