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
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Luminosity.IO.Examples {
    public class PlayerScript : MonoBehaviour {
        public PlayerID m_playerID = PlayerID.One;

        [Tooltip("Stat")]
        public int powerLoaded = 0;
        public float MovementSpeed;
        public float staminaLeft = 100;
        public float rotationSpeed = 5;
        public float ShootingRate;
        public int ShootingDamage;
        public int HealthPoint;
        private int maxHealthPoint;
        public int AmmoCount;
        private int maxAmmoCount;

        private const float minigunTime = 7.5f;
        private float minigunLeft = 0;

        [Tooltip("Object")]
        public GameObject PlayerBullet;
        public GameObject miniGun;
        public GameObject rocketGun;
        public GameObject iceGun;
        public AudioClip ShootingAudioClip;
        public GameObject firePoint;
        public GameObject firePointLight;
        public Image hitEffect;

        private bool isRunning;
        private bool recentlyExhausted;
        private float exhaustedTimer = 2;
        private bool onHit = false;
        private Rigidbody rb = null;
        private Vector3 moveDirection = Vector3.zero;
        private bool canShoot;
        private AudioSource audioSource;
        private Animator anim;
        private GameObject bullet;

        private int ControllerSupported = 0;

        void Start() {
            rb = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();
            anim = GetComponentInChildren<Animator>();

            canShoot = true;
            audioSource.clip = ShootingAudioClip;

            maxAmmoCount = AmmoCount;
            maxHealthPoint = HealthPoint;
            GameManager.Instance.UpdateAmmo(AmmoCount);
            GameManager.Instance.UpdateHealth(HealthPoint);
        }

        void Update() {
            if (powerLoaded == 1) {
                minigunLeft += Time.deltaTime;
                ShootingRate = 0.05f;
                if (minigunLeft > minigunTime) {
                    ShootingRate = 0.15f;
                    powerLoaded = 0;
                    minigunLeft = 0;
                }
            }

            if (GameManager.Instance.isGameOver) {
                GetComponentInChildren<Animator>().enabled = false;
                return;
            }
            if (!GameManager.isPaused) {
                UpdateMovement();
                UpdateRotation();
                Shoot();
            }

            string[] names = Input.GetJoystickNames();
            for (int x = 0; x < names.Length; x++) {
                if (names[x].Length >= 19) {
                    print("CONTROLLER IS CONNECTED");
                    ControllerSupported = 1;
                }
            }

            if (onHit) {
                if (hitEffect.color.a < 255) {
                    hitEffect.color += new Color(255, 255, 255, Time.deltaTime * 5);
                    if (hitEffect.color.a > 1) {
                        onHit = false;
                    }
                }
            }
            else {
                hitEffect.color -= new Color(255, 255, 255, Time.deltaTime * 7);
                onHit = false;
            }
        }

        private void UpdateMovement() {
            // Get the direction based on the user input
            moveDirection = new Vector3(InputManager.GetAxis("Horizontal", m_playerID), 0, InputManager.GetAxis("Vertical", m_playerID));
            moveDirection.Normalize();

            // Set the velocity to the direction * movement speed
            rb.velocity = new Vector3(moveDirection.x * MovementSpeed,
                                      rb.velocity.y,
                                      moveDirection.z * MovementSpeed);

            if (ControllerSupported == 1) {
                if (rb.velocity.magnitude <= 1.6f) {
                    anim.SetBool("Walk", false);
                    anim.SetBool("Run", false);

                    if (!InputManager.GetButton("Sprint", m_playerID) || InputManager.GetButton("Sprint", m_playerID))
                        isRunning = false;
                }
                else if (rb.velocity.magnitude >= 1.7f) {
                    anim.SetBool("Walk", true);

                    if (InputManager.GetButton("Sprint", m_playerID)) {
                        if (staminaLeft > 0 && !recentlyExhausted)
                            isRunning = true;
                        else if (recentlyExhausted)
                            isRunning = false;
                    }
                    else if (!InputManager.GetButton("Sprint", m_playerID)) {
                        isRunning = false;
                    }
                }
            }
            else {
                if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) {
                    anim.SetBool("Walk", false);
                    anim.SetBool("Run", false);

                    if (!Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift))
                        isRunning = false;
                }
                else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
                    anim.SetBool("Walk", true);

                    if (Input.GetKey(KeyCode.LeftShift)) {
                        if (staminaLeft > 0 && !recentlyExhausted)
                            isRunning = true;
                        else if (recentlyExhausted)
                            isRunning = false;
                    }
                    else if (!Input.GetKey(KeyCode.LeftShift)) {
                        isRunning = false;
                    }
                }
            }

            if (isRunning) {
                if (staminaLeft < 1) {
                    recentlyExhausted = true;
                }

                anim.SetBool("Run", true);
                staminaLeft -= Time.deltaTime * 10;
                MovementSpeed = 2.8f; // 2.8f
                GameManager.Instance.StaminaLeftTextbox.text = "Stamina : " + (int)staminaLeft;
                GameManager.Instance.staminaSlider.localScale = new Vector3(staminaLeft / 100, 1, 1);
            }
            else {
                if (recentlyExhausted) {
                    if (exhaustedTimer > 0) {
                        exhaustedTimer -= Time.deltaTime;
                        if (exhaustedTimer <= 0) {
                            exhaustedTimer = 2;
                            recentlyExhausted = false;
                        }
                    }
                }
                anim.SetBool("Run", false);
                staminaLeft += Time.deltaTime * 10;
                MovementSpeed = 1.8f; // 1.8f
                GameManager.Instance.StaminaLeftTextbox.text = "Stamina : " + (int)staminaLeft;
                GameManager.Instance.staminaSlider.localScale = new Vector3(staminaLeft / 100, 1, 1);
            }
            staminaLeft = Mathf.Clamp(staminaLeft, 0, 100);
        }

        private void UpdateRotation() {
            // The step size is dependent on the delta time.
            float step = MovementSpeed * rotationSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, moveDirection, step, 0.0f);

            // Rotate our position a step closer to the target.
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        private void Shoot() {
            if (InputManager.GetButton("Fire1", m_playerID) && canShoot && AmmoCount > 0) {
                StartCoroutine(SpawnBullet());
            }
        }

        private IEnumerator SpawnBullet() {
            StartCoroutine(FirePointLight(0.1f));

            //GameObject bullet = Instantiate(PlayerBullet, transform.position + transform.forward, Quaternion.identity);
            if (powerLoaded == 1) {
                bullet = Instantiate(miniGun, firePoint.transform.position, Quaternion.identity);
            }
            else if (powerLoaded == 2) {
                bullet = Instantiate(rocketGun, firePoint.transform.position, Quaternion.identity);
                powerLoaded = 0;
            }
            else if (powerLoaded == 3) {
                bullet = Instantiate(iceGun, firePoint.transform.position, Quaternion.identity);
                powerLoaded = 0;
            }
            else {
                bullet = Instantiate(PlayerBullet, firePoint.transform.position, Quaternion.identity);
            }

            bullet.transform.forward = transform.forward;
            bullet.GetComponent<BulletScript>().BulletDamage = ShootingDamage;
            audioSource.PlayOneShot(ShootingAudioClip, 0.5f);

            GameManager.Instance.ammoSlider.localScale = new Vector3((float)AmmoCount / maxAmmoCount, 1, 1);
            GameManager.Instance.UpdateAmmo(--AmmoCount);


            canShoot = false;
            //wait for some time
            yield return new WaitForSeconds(ShootingRate);

            canShoot = true;
        }

        private IEnumerator FirePointLight(float burstSeconds) {
            firePointLight.SetActive(true);
            yield return new WaitForSeconds(burstSeconds);
            firePointLight.SetActive(false);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.tag.Equals("Enemy")) {
                hitEffect.color = new Color(255, 255, 255, 0);
                onHit = true;

                if (other.gameObject.name == "Boss")
                    HealthPoint -= other.gameObject.GetComponent<BossType>().ContactDamage;
                else
                    HealthPoint -= 20;

                GameManager.Instance.healthSlider.localScale = new Vector3((float)HealthPoint / maxHealthPoint, 1, 1);
                GameManager.Instance.UpdateHealth(HealthPoint);

                if (HealthPoint <= 0)
                    Dead();
            }
        }

        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject.CompareTag("Enemy")) {
                hitEffect.color = new Color(255, 255, 255, 0);
                onHit = true;

                if (collision.gameObject.name == "Enemy(Clone)")
                    HealthPoint -= collision.gameObject.GetComponent<EnemyScript>().ContactDamage;

                GameManager.Instance.healthSlider.localScale = new Vector3((float)HealthPoint / maxHealthPoint, 1, 1);
                GameManager.Instance.UpdateHealth(HealthPoint);

                if (HealthPoint <= 0)
                    Dead();
            }
        }

        private void Dead() {
            Luminosity.IO.Examples.MainMenu.mainMenu.GameOverUI();
            Destroy(gameObject);
        }

        public void AddAmmo(int ammo, AudioClip audioClip) {
            maxAmmoCount += ammo;
            AmmoCount += ammo;

            GameManager.Instance.ammoSlider.localScale = new Vector3((float)AmmoCount / maxAmmoCount, 1, 1);
            GameManager.Instance.UpdateAmmo(AmmoCount);

            audioSource.PlayOneShot(audioClip);
        }

        public void AddHealth(int health, AudioClip audioClip) {
            maxHealthPoint += health;
            HealthPoint += health;

            GameManager.Instance.healthSlider.localScale = new Vector3((float)HealthPoint / maxHealthPoint, 1, 1);
            GameManager.Instance.UpdateHealth(HealthPoint);

            audioSource.PlayOneShot(audioClip);
        }
    }
}