using System.Collections;
using System.Collections.Generic;
using Luminosity.IO.Examples;
using UnityEngine;

public class ForceField : MonoBehaviour {

    public float fieldHealth = 500;

    private void Start () { }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Bullet")) {
            if (gameObject.name == "BossShield")
                Destroy(other.gameObject);
            else
                fieldHealth -= 25;


            if (fieldHealth <= 0)
                Destroy(gameObject);
        }
    }
}
