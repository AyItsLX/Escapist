using System.Collections;
using System.Collections.Generic;
using Luminosity.IO.Examples;
using UnityEngine;
using UnityEngine.UI;

public class BossAttribute : MonoBehaviour {
    [Header("Attribute")]
    public GameObject BossObj;

    [Header("User Interface")]
    public Transform bossHealthSlider;
    public Text bossHealthText;

    // Private Variables
    private BossType bossScript;
    private int bossMaxHealth;

	void Start () {
        BossObj.SetActive(false);
        bossHealthSlider.parent.gameObject.SetActive(false);

        bossScript = BossObj.GetComponent<BossType>();
        bossMaxHealth = bossScript.HealthPoint;
    }
	
	void Update () {
        if (BossObj != null) {
            bossHealthText.text = "Boss Health: " + bossScript.HealthPoint;
            bossHealthSlider.localScale = new Vector3((float)bossScript.HealthPoint / bossMaxHealth, 1, 1);
        }

        if (bossScript.HealthPoint <= 0 && GameManager.Instance.isGameOver == false) {
            Luminosity.IO.Examples.MainMenu.mainMenu.GameOverUI();
            GameManager.Instance.SetGameOver(true);
        }
	}

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            BossObj.SetActive(true);
            bossHealthSlider.parent.gameObject.SetActive(true);
        }
    }
}
