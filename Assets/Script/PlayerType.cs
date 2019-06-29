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
using UnityEngine.AI;

namespace Luminosity.IO.Examples {
    public class PlayerType : MonoBehaviour {
        public static PlayerType playerInstance;
        public PlayerID m_playerID = PlayerID.One;

        [Header("Stat")]
        public int health;
        public int damage;
        public int ammoCount;
        public float sprintSpeed;
        public float movementSpeed;
        public float shootingRate;
        public float staminaLeft = 100;
        [HideInInspector] public int staminaRegen = 1;

        [HideInInspector] public int maxHealthPoint;
        [HideInInspector] public int maxAmmoCount;

        private bool canShoot;
        private bool onHit = false;

        private bool isRunning;
        private bool recentlyExhausted;
        private float exhaustedTimer = 2;

        [Header("Object")]
        public GameObject PlayerBullet;
        public GameObject firePoint;
        public GameObject firePointLight;
        public AudioClip ShootingAudioClip;
        public Image hitVFXSprite;

        private Vector3 moveDirection = Vector3.zero;

        private Animator anim;
        private Rigidbody rb;
        private AudioSource audioSource;
        private NavMeshAgent nma;

        private void Awake() {
            rb = GetComponent<Rigidbody>();
            nma = GetComponent<NavMeshAgent>();
            audioSource = GetComponent<AudioSource>();
            anim = GetComponentInChildren<Animator>();

            nma.speed = sprintSpeed;
            audioSource.clip = ShootingAudioClip;
            canShoot = true;

            maxAmmoCount = ammoCount;
            maxHealthPoint = health;
        }

        private void Start() { }

        private void Update() {
            if (GUIManager.GUIManager_Instance.gameOver) {
                anim.enabled = false;
                return;
            }

            if (!GUIManager.isPaused && !GUIManager.GUIManager_Instance.isInMenu) {
                UpdateMovement();
                UpdateRotation();
                Shoot();
            }
        }

        IEnumerator HitVFX() {
            const float waitTime = 3f;
            float counter = 0f;

            Debug.Log("Hello Before Waiting");
            while (counter < waitTime) {
                Debug.Log("Current WaitTime: " + counter);
                counter += Time.deltaTime;
                yield return null; //Don't freeze Unity
            }
            Debug.Log("Hello After waiting for 3 seconds");
        }

        private void UpdateMovement() {
            // Get the direction based on the user input
            moveDirection = new Vector3(InputManager.GetAxis("Horizontal", m_playerID), 0, InputManager.GetAxis("Vertical", m_playerID));
            moveDirection.Normalize();

            // Set the velocity to the direction * movement speed
            rb.velocity = new Vector3(moveDirection.x * sprintSpeed,
                                      rb.velocity.y,
                                      moveDirection.z * sprintSpeed);

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

            staminaLeft = Mathf.Clamp(staminaLeft, 0, 100);

            if (isRunning) {
                if (staminaLeft < 1) {
                    recentlyExhausted = true;
                }

                anim.SetBool("Run", true);
                staminaLeft -= Time.deltaTime * 10;
                movementSpeed = sprintSpeed;
                GUIManager.GUIManager_Instance.UpdateStamina();
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
                staminaLeft += Time.deltaTime * staminaRegen;
                movementSpeed = 1.8f;
                GUIManager.GUIManager_Instance.UpdateStamina();
            }
        }

        private void UpdateRotation() {
            // The step size is dependent on the delta time.
            float step = sprintSpeed * 8 * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, moveDirection, step, 0.0f);

            // Rotate our position a step closer to the target.
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject.tag.Equals("Enemy")) {

                //onHit = true;

                EnemyType enemyType = collision.gameObject.GetComponent<EnemyType>();
                health -= enemyType.enemyDamage;

                GUIManager.GUIManager_Instance.UpdateHealth();
                if (health <= 0) {
                    Destroy(gameObject);
                }
            }
        }

        private void Shoot() {
            if (InputManager.GetButton("Fire1", m_playerID) && canShoot && ammoCount > 0) {
                StartCoroutine(SpawnBullet());
            }
        }

        private IEnumerator SpawnBullet() {
            StartCoroutine(FirePointLight(0.1f));
            GameObject bullet = Instantiate(PlayerBullet, firePoint.transform.position, Quaternion.identity);

            bullet.transform.forward = transform.forward;
            bullet.GetComponent<BulletType>().BulletDamage = damage;
            audioSource.PlayOneShot(ShootingAudioClip, 0.5f);

            --ammoCount;
            GUIManager.GUIManager_Instance.UpdateBullet();
            canShoot = false;
            yield return new WaitForSeconds(shootingRate);
            canShoot = true;
        }

        private IEnumerator FirePointLight(float burstSeconds) {
            firePointLight.SetActive(true);
            yield return new WaitForSeconds(burstSeconds);
            firePointLight.SetActive(false);
        }

        public void AddAmmo(int ammo, AudioClip audioClip) {
            maxAmmoCount += ammo;
            ammoCount += ammo;

            GUIManager.GUIManager_Instance.UpdateBullet();
            audioSource.PlayOneShot(audioClip);
        }

        public void AddHealth(int health, AudioClip audioClip) {
            maxHealthPoint += health;
            this.health += health;

            GUIManager.GUIManager_Instance.UpdateHealth();
            audioSource.PlayOneShot(audioClip);
        }

        //hitEffect.color = new Color(255, 255, 255, 0);
        //if (onHit) {
        //    if (hitEffect.color.a < 255) {
        //        hitEffect.color += new Color(255, 255, 255, Time.deltaTime * 5);
        //        if (hitEffect.color.a > 1) {
        //            onHit = false;
        //        }
        //    }
        //}
        //else {
        //    hitEffect.color -= new Color(255, 255, 255, Time.deltaTime * 7);
        //    onHit = false;
        //}

        //if (Xbox_One_Controller == 1) {
        //    if (rb.velocity.magnitude >= 1.7f) {
        //        print("Running");
        //        anim.SetBool("Walk", true);

        //        if (Input.GetKey(KeyCode.JoystickButton6)) {
        //            anim.SetBool("Run", true);
        //            MovementSpeed = 2.8f;
        //        }
        //        else if (!Input.GetKey(KeyCode.JoystickButton6)) {
        //            anim.SetBool("Run", false);
        //            MovementSpeed = 1.8f;
        //        }
        //    }
        //    else if (rb.velocity.magnitude <= 1.6f) {
        //        anim.SetBool("Walk", false);
        //        anim.SetBool("Run", false);
        //    }
        //}
        //else {
        //}

        //string[] names = Input.GetJoystickNames();
        //    for (int x = 0; x<names.Length; x++) {
        //        //print(names[x].Length);
        //        if (names[x].Length == 19) {
        //            print("XBOX ONE CONTROLLER IS CONNECTED");
        ////set a controller bool to true
        //Xbox_One_Controller = 1;
        //        }
        //    }

        //IEnumerator waitFunction2() {
        //    const float waitTime = 3f;
        //    float counter = 0f;

        //    Debug.Log("Hello Before Waiting");
        //    while (counter < waitTime) {
        //        Debug.Log("Current WaitTime: " + counter);
        //        counter += Time.deltaTime;
        //        yield return null; //Don't freeze Unity
        //    }
        //    Debug.Log("Hello After waiting for 3 seconds");
        //}

        //IEnumerator HitVFX() {
        //    float a = 5;
        //    while (a > 0) {
        //        a -= Time.deltaTime;
        //        print("A In While Loop: " + a);
        //    }
        //    yield break;
        //}
    }
}