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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Luminosity.IO.Examples {
    public class GUIManager : MonoBehaviour {
        public static GUIManager GUIManager_Instance;
        public static bool isPaused = false;
        public static int amountOfPlayer = 1;

        [HideInInspector] public bool isInMenu = false;
        [HideInInspector] public bool usingUpgrade = false;
        [HideInInspector] public bool usingSkill = false;

        [Header("Game Stat")]
        public float experienceCount;
        public float goldCount;
        public float gameTime = 0;
        public bool gameOver = false;

        public static float increasedEnemyHealth = 100;

        [Header("Player 1")]
        public PlayerType p1_GameObject;
        public Text p1_HealthText;
        public Transform p1_HealthSlider;
        public Text p1_StaminaText;
        public Transform p1_StaminaSlider;
        public Text p1_BulletText;
        public GameObject Upgrade1;
        public GameObject Skill1;

        [Header("Player 2")]
        public PlayerType p2_GameObject;
        public Text p2_HealthText;
        public Transform p2_HealthSlider;
        public Text p2_StaminaText;
        public Transform p2_StaminaSlider;
        public Text p2_BulletText;
        public GameObject Upgrade2;
        public GameObject Skill2;

        [Header("Objective")]
        public Text statUI;
        public Text objectiveHealthText;
        public Transform objectiveHealthSlider;
        public Text goldText;
        public Transform goldSlider;
        public Text experienceText;
        public Transform experienceSlider;

        [Header("Object")]
        public GameObject PauseUI;
        private void Awake() {
            if (GUIManager_Instance == null)
                GUIManager_Instance = this;
        }

        private void Start() {
            PauseUI.SetActive(false);

            UpdateExperience();
            UpdateGold();
        }

        private void Update() {
            if (InputManager.GetButtonDown("PauseMenu", PlayerID.One)) {
                if (!isPaused) {
                    isInMenu = false;
                    usingUpgrade = false;
                    usingSkill = false;
                    Luminosity.IO.Examples.MainMenu.mainMenu.ChangePage("Pause");
                    Upgrade1.SetActive(false);
                    Skill1.SetActive(false);

                    isPaused = true;
                    PauseUI.SetActive(true);
                }
                else {
                    isPaused = false;
                    PauseUI.SetActive(false);
                }
            }

            if (!isPaused) {
                UpdateStatUI();
                increasedEnemyHealth += Time.deltaTime * 5;

                if (InputManager.GetButtonDown("Upgrade", PlayerID.One) && !usingSkill) {
                    if (GUIManager_Instance.Upgrade1.activeSelf) {
                        isInMenu = false;
                        usingUpgrade = false;
                        Upgrade1.SetActive(false);
                    }
                    else {
                        isInMenu = true;
                        usingUpgrade = true;
                        Luminosity.IO.Examples.MainMenu.mainMenu.ChangePage("Upg1");
                        Upgrade1.SetActive(true);
                    }
                }

                if (InputManager.GetButtonDown("Skill", PlayerID.One) && !usingUpgrade) {
                    if (GUIManager.GUIManager_Instance.Skill1.activeSelf) {
                        isInMenu = false;
                        usingSkill = false;
                        Skill1.SetActive(false);
                    }
                    else {
                        isInMenu = true;
                        usingSkill = true;
                        Luminosity.IO.Examples.MainMenu.mainMenu.ChangePage("Skill1");
                        Skill1.SetActive(true);
                    }
                }
            }
        }

        public void UpdateStatUI() {
            statUI.text = "Damage: "+p1_GameObject.damage+ "\nAttackSpeed: " + p1_GameObject.shootingRate + "\nStaminaRegen: " + p1_GameObject.staminaRegen + "\nMoveSpeed: " + p1_GameObject.sprintSpeed + "";
        }

        public void UpdateExperience() {
            experienceText.text = "Experience: " + experienceCount;
            experienceSlider.localScale = new Vector3(experienceCount / 1000, 1, 1);
        }

        public void UpdateGold() {
            goldText.text = "Gold: " + goldCount;
            goldSlider.localScale = new Vector3(goldCount / 1000, 1, 1);
        }

        public void UpdateHealth() {
            p1_HealthText.text = "Health: " + p1_GameObject.health;
            p1_HealthSlider.localScale = new Vector3((float)p1_GameObject.health / 100, 1, 1);

            p2_HealthText.text = "Health: " + p2_GameObject.health;
            p2_HealthSlider.localScale = new Vector3((float)p2_GameObject.health / 100, 1, 1);
        }

        public void UpdateStamina() {
            p1_StaminaText.text = "Stamina: " + (int)p1_GameObject.staminaLeft;
            p1_StaminaSlider.localScale = new Vector3(p1_GameObject.staminaLeft / 100, 1, 1);

            p2_StaminaText.text = "Stamina: " + (int)p2_GameObject.staminaLeft;
            p2_StaminaSlider.localScale = new Vector3(p2_GameObject.staminaLeft / 100, 1, 1);
        }

        public void UpdateBullet() {
            p1_BulletText.text = "Ammo: " + p1_GameObject.ammoCount;

            p2_BulletText.text = "Ammo: " + p2_GameObject.ammoCount;
        }

        public void OnResumePressed() {
            isPaused = false;
            PauseUI.SetActive(false);
        }

        public void ResetGame() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void OnMainMenuPressed() {
            GameManager.isPaused = false;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
