using System.Collections;
using System.Collections.Generic;
using Luminosity.IO.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    
    public GameObject lockMenuMain;
    public GameObject lockMenuInstruction;
    public GameObject lockMenuSetting;
    public GameObject lockMenuLevel;

    public GameObject Lv2;
    public GameObject Lv3;
    public Color clickableColor;

    public Transition transition;
    public Dropdown resolutionDropdown;

    Resolution[] resolutions;
    
    int menuPos = 0;
    int prevPos;
    NavMeshAgent playerObj;
    NavMeshAgent enemyObj;
    Vector3 movePos;
    float resetTime;
    AudioSource audioSource;

    void Start () {
        audioSource = gameObject.GetComponent<AudioSource>();

        if (PlayerPrefs.GetInt("LevelCleared") >= 3) {
            Lv3.GetComponent<Image>().color = clickableColor;
            Lv3.GetComponent<Button>().interactable = true;
        }
        else {
            Lv3.GetComponent<Image>().color = new Color(255, 0, 0);
            Lv3.GetComponent<Button>().interactable = false;
        }
        if (PlayerPrefs.GetInt("LevelCleared") >= 2) {
            Lv2.GetComponent<Image>().color = clickableColor;
            Lv2.GetComponent<Button>().interactable = true;
        }
        else {
            Lv2.GetComponent<Image>().color = new Color(255, 0, 0);
            Lv2.GetComponent<Button>().interactable = false;
        }

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++) {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height) {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        if (SceneManager.GetActiveScene().name == "MainMenu") {
            playerObj = GameObject.Find("Player").GetComponent<NavMeshAgent>();
            enemyObj = GameObject.Find("Enemy").GetComponent<NavMeshAgent>();
            movePos = new Vector3(Random.Range(-2f, 2.75f), 0, Random.Range(0f, 4.75f));
        }
        print("LevelCleared : " + PlayerPrefs.GetInt("LevelCleared"));
    }

    void Update () {
        ChasePlayer();

        if (menuPos == 0 && prevPos == 1 && transform.localPosition.x > 0) {
            transform.localPosition -= new Vector3(Time.deltaTime * 3000, 0, 0);
        }
        else if (menuPos == 0 && prevPos == -1 && transform.localPosition.x < 0) {
            transform.localPosition += new Vector3(Time.deltaTime * 3000, 0, 0);
        }
        else if (menuPos == 0 && prevPos == 2 && transform.localPosition.y > 0) {
            transform.localPosition -= new Vector3(0, Time.deltaTime * 3000, 0);
        }
        else if (menuPos == 1 && transform.localPosition.x < 2000) {
            transform.localPosition += new Vector3(Time.deltaTime * 3000, 0, 0);
        }
        else if (menuPos == -1 && transform.localPosition.x > -2000) {
            transform.localPosition -= new Vector3(Time.deltaTime * 4000, 0, 0);
        }
        else if (menuPos == 2 && transform.localPosition.y < 1000) {
            transform.localPosition += new Vector3(0, Time.deltaTime * 3000, 0);
        }
    }

    #region ChasePlayer
    void ChasePlayer() {
        resetTime += Time.deltaTime;
        if (resetTime >= 2) {
            resetTime = 0;

            movePos = new Vector3(Random.Range(-2f, 2.75f), 0, Random.Range(0f, 4.75f));
        }

        if (Vector3.Distance(playerObj.transform.position, enemyObj.transform.position) < 0.6) {
            Vector3 direction = playerObj.transform.position - enemyObj.transform.position;
            playerObj.GetComponent<Rigidbody>().AddForceAtPosition(direction * 50, enemyObj.transform.position);
        }

        playerObj.SetDestination(movePos);
        enemyObj.SetDestination(playerObj.gameObject.transform.position);

        if (!playerObj.isStopped)
            playerObj.GetComponentInChildren<Animator>().SetBool("Walk", true);
        else
            playerObj.GetComponentInChildren<Animator>().SetBool("Walk", false);

        if (!enemyObj.isStopped)
            enemyObj.GetComponentInChildren<Animator>().SetBool("Run", true);
        else
            enemyObj.GetComponentInChildren<Animator>().SetBool("Run", false);
    }
    #endregion

    #region Settings
    public void SetVolume(float volume) {
        AudioListener.volume = volume;
    }

    public void SetQuality(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen) {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution (int resolutionIndex) {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    #endregion

    #region Buttons
    public void OnCooperativePressed() {
        SceneManager.LoadScene("Survival Mode");
    }

    public void OnStartPressed() {
        transition.gameObject.SetActive(true);
        transition.sceneName = "Start";
    }

    public void OnLevelSelectPressed() {
        menuPos = 2;
        lockMenuMain.SetActive(true);
        lockMenuLevel.SetActive(false);
    }

    public void OnQuitPressed() {
        Application.Quit();
    }

    public void OnSettingPressed() {
        menuPos = -1;
        lockMenuMain.SetActive(true);
        lockMenuSetting.SetActive(false);
    }

    public void OnInstructionPressed() {
        menuPos = 1;
        lockMenuMain.SetActive(true);
        lockMenuInstruction.SetActive(false);
    }

    public void OnRestartPressed() {
        GameManager.isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnBackPressed() {
        if (menuPos == 1) {
            prevPos = 1;
            lockMenuInstruction.SetActive(true);
        }
        else if (menuPos == 2) {
            prevPos = 2;
            lockMenuLevel.SetActive(true);
        }
        else {
            prevPos = -1;
            lockMenuSetting.SetActive(true);
        }

        menuPos = 0;
        lockMenuMain.SetActive(false);
    }

    public void OnMainMenuPressed() {
        GameManager.isPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnLevelsSelected(string levelName) {
        transition.gameObject.SetActive(true);
        transition.sceneName = levelName;
    }

    public void OnRestartProgressPressed() {
        PlayerPrefs.SetInt("LevelCleared", 1);
        print("Resetted LevelCleared : " + PlayerPrefs.GetInt("LevelCleared"));

        if (PlayerPrefs.GetInt("LevelCleared") >= 3) {
            Lv3.GetComponent<Image>().color = clickableColor;
            Lv3.GetComponent<Button>().interactable = true;
        }
        else {
            Lv3.GetComponent<Image>().color = new Color(255, 0, 0);
            Lv3.GetComponent<Button>().interactable = false;
        }
        if (PlayerPrefs.GetInt("LevelCleared") >= 2) {
            Lv2.GetComponent<Image>().color = clickableColor;
            Lv2.GetComponent<Button>().interactable = true;
        }
        else {
            Lv2.GetComponent<Image>().color = new Color(255, 0, 0);
            Lv2.GetComponent<Button>().interactable = false;
        }
    }

    public void OnNextPressed() {
        PlayerPrefs.SetInt("LevelCleared", SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    #endregion
}
