using System.Collections;
using System.Collections.Generic;
using Luminosity.IO.Examples;
using UnityEngine.UI;
using UnityEngine;

public class Achievement : MonoBehaviour {
    public static Achievement achievementInstance;
    public PlayerScript playerScript;

    public Text achievementText;
    public GameObject achievementObj;
    
    public static int outOfAmmo; // out of ammo
    public static int firstPower; // first power up
    public static int minionKills; // 1, 10, 100
    public static int bossKills; // killed the boss

    private float timer = 5;

    private void Awake() {
        if (achievementInstance == null)
            achievementInstance = this;

        playerScript = GameObject.Find("Player").GetComponent<PlayerScript>();
    }

    void Start () {

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
            StartCoroutine(ShowText("Out Of Ammo", "Did you really?"));
        }
        else if (Input.GetKeyDown(KeyCode.Y)) {
            StartCoroutine(ShowText("PowerUp", "Perfect Balance"));
        }
        else if (Input.GetKeyDown(KeyCode.U)) {
            StartCoroutine(ShowText("Minions Killed", "10 Minion Killed"));
        }
        else if (Input.GetKeyDown(KeyCode.I)) {
            StartCoroutine(ShowText("Boss Killed", "Nicely Done"));
        }

        CheckAchievement();
    }

    void CheckAchievement() {
        if (playerScript.AmmoCount <= 0 && outOfAmmo != 1) {
            outOfAmmo = 1;
            StartCoroutine(ShowText("Out Of Ammo","Did you really?"));
        }

        if (firstPower == 1 && firstPower != 2) {
            firstPower = 2;
            StartCoroutine(ShowText("PowerUp", "Perfect Balance"));
        }

        if (minionKills == 10 && minionKills != 11) {
            minionKills = 11;
            StartCoroutine(ShowText("Minions Killed", "10 Minion Killed"));
        }

        if (bossKills == 1 && bossKills != 2) {
            bossKills = 2;
            StartCoroutine(ShowText("Boss Killed", "Nicely Done"));
        }
    }

    IEnumerator ShowText(string achievementTitle ,string achievementDescription) {
        achievementObj.SetActive(true);
        achievementText.text = achievementTitle + "\n\n"+achievementDescription;
        yield return new WaitForSeconds(3);
        achievementObj.SetActive(false);
    }
}
