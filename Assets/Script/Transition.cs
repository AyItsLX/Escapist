using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour {

    public string sceneName;

	void Start () {}
	
    public void SceneTransition() {
        if (sceneName == "Start") {
            if (PlayerPrefs.GetInt("LevelCleared") <= 1) {
                PlayerPrefs.SetInt("LevelCleared", 1);
            }
            SceneManager.LoadScene("Lv1");
        }
        else if (sceneName == "Lv1") {
            SceneManager.LoadScene("Lv1");
        }
        else if (sceneName == "Lv2") {
            SceneManager.LoadScene("Lv2");
        }
        else if (sceneName == "Lv3") {
            SceneManager.LoadScene("Lv3");
        }
    }

    public void TurnOff() {
        gameObject.SetActive(false);
    }
}
