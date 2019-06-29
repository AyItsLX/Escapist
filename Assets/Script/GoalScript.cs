using System.Collections;
using System.Collections.Generic;
using Luminosity.IO.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoalScript : MonoBehaviour {
    private float AnimateSpeed = 100;
    public GameObject nextButton;
    public Color buttonColor;

    [HideInInspector]
    public bool lv1Ending;
    [HideInInspector]
    public bool lv2Ending;

    private CameraScript cameraScript;

    void Start() {
        cameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
    }

    void Update() {
        transform.Rotate(Vector3.up, AnimateSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag.Equals("Player") && !GameManager.Instance.isGameOver) {
            if (SceneManager.GetActiveScene().name == "Lv1") {
                cameraScript.player = null;
                lv1Ending = true;
            }
            else if (SceneManager.GetActiveScene().name == "Lv2") {
                lv2Ending = true;
            }

            nextButton.GetComponent<Image>().color = buttonColor;
            nextButton.GetComponent<Button>().interactable = true;

            if (SceneManager.GetActiveScene().name == "Lv1") {
                PlayerPrefs.SetInt("LevelCleared", 2);
            }
            else if (SceneManager.GetActiveScene().name == "Lv2") {
                PlayerPrefs.SetInt("LevelCleared", 3);
            }

            Luminosity.IO.Examples.MainMenu.mainMenu.GameOverUI();
            GameManager.Instance.SetGameOver(true);
        }
    }
}
