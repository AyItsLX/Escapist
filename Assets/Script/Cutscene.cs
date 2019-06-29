using System.Collections;
using System.Collections.Generic;
using Luminosity.IO.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour {

    public float lerpSpeed = 2;

    [Header("Level 3")]
    public GameObject explosion;
    public GameObject ship;

    [Header("Level 2")]
    public GameObject explosionObj;
    public Transform EndingLift;
    public Transform StartingLift;

    [Header("Level 1")]
    public GameObject helicopterObj;
    public GameObject shipObj;
    public Transform shipCameraPos;

    [Header("ETC")]
    public GameObject endGoal;

    private bool startLerpLv1, startGame;
    private bool startLerpLv2;
    private bool startLerpLv3;

    private AudioSource audioSource;
    private GameObject playerObj;
    private CameraScript cameraScript;
    private GameManager gameManager;
   
	void Start () {
        audioSource = GetComponent<AudioSource>();
        playerObj = GameObject.Find("Player");
        cameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (SceneManager.GetActiveScene().name == "Lv1") {
            playerObj.SetActive(false);
            cameraScript.player = helicopterObj;
        }
        else if (SceneManager.GetActiveScene().name == "Lv3") {
            StartCoroutine(StartingExplosion());
        }
    }
	
	void Update () {
        if (SceneManager.GetActiveScene().name == "Lv2") {
            if (StartingLift.transform.position.y > 0.4f) {
                StartingLift.transform.position -= new Vector3(0, 0.75f, 0) * Time.deltaTime;
            }
            else if (endGoal.GetComponent<GoalScript>().lv2Ending && StartingLift.transform.position.y < 3) {
                EndingLift.transform.position += new Vector3(0, 0.75f, 0) * Time.deltaTime;
                explosionObj.SetActive(true);
            }
        }

        if (SceneManager.GetActiveScene().name == "Lv1") {
            if (startLerpLv1) {
                cameraScript.transform.position = Vector3.Lerp(cameraScript.gameObject.transform.position, new Vector3(0, 2.5f, -3), Time.deltaTime * lerpSpeed);
                if (Vector3.Distance(cameraScript.gameObject.transform.position, new Vector3(0, 2.5f, -3)) < 0.1f) {
                    gameManager.hasStarted = true;
                    playerObj.GetComponent<PlayerScript>().enabled = true;
                    cameraScript.player = playerObj;
                    startGame = true;
                    startLerpLv1 = false;
                }
            }
            else if (startGame) {
                helicopterObj.GetComponent<AudioSource>().volume -= Time.deltaTime * 0.25f;
                if (helicopterObj.GetComponent<AudioSource>().volume <= 0) {
                    gameManager.audioSource.Play();
                    startGame = false;
                }
            }
            else if (endGoal.GetComponent<GoalScript>().lv1Ending) {
                if (cameraScript.transform.position.y < 4f) {
                    cameraScript.transform.position += new Vector3(-0.5f, 1, -1) * Time.deltaTime * 0.5f;
                    shipObj.transform.position += new Vector3(1, 0, -1) * Time.deltaTime * 0.5f;
                }
                gameManager.hasStarted = false;
                playerObj.GetComponent<PlayerScript>().enabled = false;
                playerObj.SetActive(false);
            }
        }
    }

    IEnumerator StartingExplosion() {
        yield return new WaitForSeconds(3f);
        explosion.SetActive(true);
        ship.GetComponent<Animator>().enabled = true;
        yield return new WaitForSeconds(4f);
        ship.SetActive(false);
    }

    public void ChangeCameraPlayer() {
        audioSource.Play();
        startLerpLv1 = true;
        playerObj.SetActive(true);
        cameraScript.player = null;
    }
}
