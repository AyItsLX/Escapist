using System.Collections;
using System.Collections.Generic;
using Luminosity.IO.Examples;
using UnityEngine;

public class PowerUpType : MonoBehaviour {

    public enum PowerType { Minigun, Rocket, Ice};
    public PowerType powerType;

	void Start () { }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerScript playerScript = other.GetComponent<PlayerScript>();

            switch (powerType) {
                case PowerType.Minigun:
                    playerScript.powerLoaded = 1;
                    break;
                case PowerType.Rocket:
                    playerScript.powerLoaded = 2;
                    break;
                case PowerType.Ice:
                    playerScript.powerLoaded = 3;
                    break;
            }

            if (Achievement.firstPower < 1)
                Achievement.firstPower++;

            Destroy(gameObject);
        }
    }
}
