#region [Copyright (c) 2018 Cristian Alexandru Geambasu]
//	Distributed under the terms of an MIT-style license:
//
//	The MIT License
//
//	Copyright (c) 2018 Cristian Alexandru Geambasu
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
//	and associated documentation files (the "Software"), to deal in the Software without restriction, 
//	including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//	and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
//	subject to the following conditions:
//
//	The above copyright notice and this permission notice shall be included in all copies or substantial 
//	portions of the Software.
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
//	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
//	FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//	ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Luminosity.IO.Examples {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance;
        public static bool isPaused = false;

        [Header("Game variables")]
        [Tooltip("Time in seconds")]
        public float RoundTime;

        [Header("Game audioClips")]
        public AudioClip BackgroundMusic;
        public AudioClip GameWinSound;
        public AudioClip GameLoseSound;

        [Header("Text boxes references")]
        public Text ScoreTextbox;
        public GameObject scoreRect;
        public string ScoreTextPrefix;

        public Text AmmoTextbox;
        public Transform ammoSlider;
        public string AmmoTextPrefix;

        public Text HealthTextbox;
        public Transform healthSlider;
        public string HealthTextPrefix;

        public Text TimeLeftTextbox;
        public Transform timeSlider;
        public string TimeLeftTextPrefix;

        public Text StaminaLeftTextbox;
        public Transform staminaSlider;

        public GameObject GameOverUI;
        public Text GameOverText;
        public GameObject PauseUI;
        public GameObject transitionObj;
        public List<GameObject> keyObj;

        [HideInInspector]
        public bool isGameOver;
        [HideInInspector]
        public bool hasStarted;
        [HideInInspector]
        public int keyNeeded;

        private int score;
        public AudioSource audioSource;

        void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = BackgroundMusic;

            GameOverUI.SetActive(false);

            if (SceneManager.GetActiveScene().name == "Lv1") {
                keyNeeded = 3;
                transitionObj.SetActive(true);
                TimeLeftTextbox.text = TimeLeftTextPrefix + ": 00:60:00";
            }
            else if (SceneManager.GetActiveScene().name == "Lv2") {
                keyNeeded = 2;
                transitionObj.SetActive(true);
                TimeLeftTextbox.text = TimeLeftTextPrefix + ": 03:00:00";
                hasStarted = true;
            }
            else if (SceneManager.GetActiveScene().name == "Lv3") {
                keyNeeded = 2;
                transitionObj.SetActive(true);
                TimeLeftTextbox.text = TimeLeftTextPrefix + ": 05:00:00";
                hasStarted = true;
            }
        }

        void Update() {
            if (isGameOver)
                return;

            if (!isPaused && hasStarted) {
                UpdateTimeLeft();
            }

            if (Input.GetKeyDown(KeyCode.Escape) || InputManager.GetButtonDown("PauseMenu", PlayerID.One)) {
                if (!isPaused) {
                    isPaused = true;
                    PauseUI.SetActive(true);
                }
                else {
                    isPaused = false;
                    PauseUI.SetActive(false);
                }
            }
        }

        public void OnResumePressed() {
            isPaused = false;
            PauseUI.SetActive(false);
        }

        public void UpdateScore(int _score, AudioClip audioClip) {
            score += _score;
            ScoreTextbox.text = ScoreTextPrefix + score;

            audioSource.PlayOneShot(audioClip);
        }

        public void UpdateAmmo(int ammo) {
            AmmoTextbox.text = AmmoTextPrefix + ammo;
        }

        public void UpdateHealth(int health) {
            if (health <= 0 && !isGameOver) {
                health = 0;
                SetGameOver(false);
            }

            HealthTextbox.text = HealthTextPrefix + health;
        }

        public void UpdateTimeLeft() {
            if (RoundTime <= 0) {
                RoundTime = 0;
                TimeLeftTextbox.text = TimeLeftTextPrefix + "00:00:00";

                Luminosity.IO.Examples.MainMenu.mainMenu.GameOverUI();
                SetGameOver(false);

                return;
            }

            RoundTime -= Time.deltaTime;

            int minutes = (int)RoundTime / 60;
            int seconds = (int)RoundTime - 60 * minutes;
            int milliseconds = (int)(100 * (RoundTime - minutes * 60 - seconds));

            if (SceneManager.GetActiveScene().name == "Lv1")
                timeSlider.localScale = new Vector3(RoundTime / 60, 1, 1);
            else if (SceneManager.GetActiveScene().name == "Lv2")
                timeSlider.localScale = new Vector3(RoundTime / 180, 1, 1);
            else if (SceneManager.GetActiveScene().name == "Lv3")
                timeSlider.localScale = new Vector3(RoundTime / 300, 1, 1);

            TimeLeftTextbox.text = TimeLeftTextPrefix + string.Format(": {0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        }

        public void SetGameOver(bool isWin) {
            isGameOver = true;
            GameOverUI.SetActive(true);

            if (isWin) {
                GameOverText.text = "Victory";
                audioSource.PlayOneShot(GameWinSound);
            }
            else {
                GameOverText.text = "Defeat";
                audioSource.PlayOneShot(GameLoseSound);
            }
        }

        public void ResetGame() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public int GetScore() {
            return score;
        }
    }
}
