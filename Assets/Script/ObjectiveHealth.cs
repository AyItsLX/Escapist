using System.Collections;
using System.Collections.Generic;
using Luminosity.IO.Examples;
using UnityEngine;

public class ObjectiveHealth : MonoBehaviour {

    public int objectiveHealth = 1000;
    private GUIManager gUIManager;

    private GameObject p1;
    private GameObject p2;

    void Start () {
        gUIManager = GUIManager.GUIManager_Instance;

        p1 = GameObject.Find("PlayerOne").gameObject;
        p1 = GameObject.Find("PlayerTwo").gameObject;
    }

    public void HitObjective(int damage) {
        objectiveHealth -= damage;

        gUIManager.objectiveHealthText.text = "Container Health: " + objectiveHealth;
        gUIManager.objectiveHealthSlider.localScale = new Vector3((float)objectiveHealth / 1000, 1, 1);

        if (objectiveHealth <= 0) {
            Destroy(p1.gameObject);
            Destroy(p2.gameObject);
            Destroy(gameObject);
        }
    }
}
