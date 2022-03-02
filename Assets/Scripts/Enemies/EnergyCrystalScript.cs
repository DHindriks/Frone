using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyCrystalScript : MonoBehaviour {
    [SerializeField]
    ParticleSystem ActiveParticle;

    [SerializeField]
    ParticleSystem DeathParticle;

    Player player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player = other.GetComponent<Player>();
            Die();
        }
    }

    void Die()
    {
        player.fuel = 10;
        ActiveParticle.Stop();
        DeathParticle.Play();
        
        //slow motion
        player.cam.GetComponent<Camerascript>().SetcamPos(player.gameObject.transform.GetChild(Random.Range(1, 3)).gameObject, gameObject, 0.75f, 0.5f, 60, 4);

        Time.timeScale = 0.25f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        Invoke("ResetTime", 0.5f);
    }

    void ResetTime ()
    {
        player.cam.GetComponent<Camerascript>().SetcamPos();
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}
