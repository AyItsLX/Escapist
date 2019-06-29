using System.Collections;
using System.Collections.Generic;
using Luminosity.IO.Examples;
using UnityEngine;
using UnityEngine.AI;

public class BossType : MonoBehaviour {
    public enum BossState {
        Charge,
        Rocket,
        Shield,
        Suicide,
    };
    public BossState bossState;

    [Header("Stat")]
    public int ContactDamage;
    public int HealthPoint;
    public int ScoreReward;
    public float MovementSpeed;
    public float chargeForce = 200;

    [SerializeField] private bool chasePlayer = false;
    [SerializeField] private bool callNextState = false;
    [SerializeField] private float timeBeforeNextState = 2;

    [Header("Object")]
    public Transform ChargeUI;
    public Transform RocketUI;
    public Transform SuicideUI;
    public GameObject bossShield;
    public GameObject DropRocket;
    public AudioClip DeathAudioClip;

    private Animator anim;
    private Rigidbody rb;
    private Transform playerTransform;
    private NavMeshAgent navMeshAgent;

    private void Start() {
        playerTransform = GameObject.Find("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        navMeshAgent.speed = MovementSpeed;

        RandomizeState(false);
    }

    private void RandomizeState(bool LowHealth) {
        if (LowHealth) {
            bossState = BossState.Suicide;
            BossAttack(bossState);
        }
        else {
            bossState = (BossState)Random.Range(0, 3);
            BossAttack(bossState);
        }
    }

    private void BossAttack(BossState bossState) {
        anim.SetBool("Run", true);
        switch (bossState) {
            case BossState.Charge:
                StartCoroutine(BossCharge());
                break;
            case BossState.Rocket:
                StartCoroutine(BossRocket());
                break;
            case BossState.Shield:
                StartCoroutine(BossShield());
                break;
            case BossState.Suicide:
                StartCoroutine(BossSuicide());
                break;
        }
    }

    IEnumerator BossCharge() {
        print("BossCharge");
        ChargeUI.gameObject.SetActive(true);
        transform.forward = (playerTransform.position - transform.position).normalized;

        while (ChargeUI.localScale.y < 1) {
            ChargeUI.localScale += new Vector3(0, 0.5f * Time.deltaTime, 0);
            yield return null;
        }
        if (ChargeUI.localScale.y > 1) {
            ChargeUI.gameObject.SetActive(false);
            rb.AddForce(transform.forward * chargeForce);

            chasePlayer = true;
            ChargeUI.localScale = new Vector3(ChargeUI.localScale.x, 0.1f, ChargeUI.localScale.z);
        }
    }

    IEnumerator BossRocket() {
        print("BossRocket");
        RocketUI.gameObject.SetActive(true);

        while (RocketUI.GetComponent<UnityEngine.UI.Image>().fillAmount < 1) {
            RocketUI.GetComponent<UnityEngine.UI.Image>().fillAmount += Time.deltaTime;
            GameObject go = Instantiate(DropRocket, new Vector3(playerTransform.position.x, 0, playerTransform.position.z), Quaternion.identity);
            Destroy(go, 2);
            yield return null;
        }
        if (RocketUI.GetComponent<UnityEngine.UI.Image>().fillAmount >= 1) {
            RocketUI.gameObject.SetActive(false);

            chasePlayer = true;
            RocketUI.GetComponent<UnityEngine.UI.Image>().fillAmount = 0;
        }
    }

    IEnumerator BossShield() {
        print("BossShield");
        bossShield.SetActive(true);

        while (bossShield.GetComponent<MeshRenderer>().material.color.a > 0) {
            bossShield.GetComponent<MeshRenderer>().material.color -= new Color(0, 0, 0, 0.25f * Time.deltaTime);
            yield return null;
        }
        if (bossShield.GetComponent<MeshRenderer>().material.color.a <= 0) {
            bossShield.SetActive(false);

            chasePlayer = true;
            bossShield.GetComponent<MeshRenderer>().material.color = new Color(bossShield.GetComponent<MeshRenderer>().material.color.r, 
                                                                               bossShield.GetComponent<MeshRenderer>().material.color.g,
                                                                               bossShield.GetComponent<MeshRenderer>().material.color.b, 0.588f);
        }
    }

    IEnumerator BossSuicide() {
        print("BossSuicide");
        SuicideUI.gameObject.SetActive(true);

        while (SuicideUI.localScale.x < 10) {
            SuicideUI.localScale += new Vector3(5 * Time.deltaTime, 5 * Time.deltaTime, 0);
            yield return null;
        }
        if (SuicideUI.localScale.x >= 10) {
            SuicideUI.gameObject.SetActive(false);

            chasePlayer = true;
            SuicideUI.localScale = new Vector3(0.2f, 0.2f, SuicideUI.localScale.z);
        }
    }

    private void Update() {
        if (playerTransform != null && !GameManager.Instance.isGameOver) {
            if (callNextState) {
                callNextState = false;
                timeBeforeNextState = 5;
                rb.velocity = Vector3.zero;

                if (HealthPoint <= 500) {
                    RandomizeState(true);
                }
                else {
                    RandomizeState(false);
                }
            }

            if (bossState != BossState.Suicide) {
                if (timeBeforeNextState > 0) {
                    timeBeforeNextState -= Time.deltaTime;

                    if (timeBeforeNextState <= 1) {
                        chasePlayer = false;
                    }
                    if (timeBeforeNextState <= 0.1f) {
                        callNextState = true;
                    }
                }
            }

            if (chasePlayer) {
                navMeshAgent.isStopped = false;
                anim.SetBool("Run", true);
                navMeshAgent.SetDestination(playerTransform.position);
            }
            else {
                navMeshAgent.isStopped = true;
                anim.SetBool("Run", false);
            }
        }

        #region Pause & Victory
        if (playerTransform == null) {
            GetComponent<Collider>().isTrigger = false;
        }

        if (GameManager.Instance.isGameOver && navMeshAgent.enabled) {
            navMeshAgent.enabled = false;
            anim.SetInteger("Victory", Random.Range(1, 3));
        }

        if (GameManager.isPaused) {
            MovementSpeed = 0;
            navMeshAgent.speed = MovementSpeed;
        }
        else if (!GameManager.isPaused) {
            MovementSpeed = 2.5f;
            navMeshAgent.speed = MovementSpeed;
        }
        #endregion
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag.Equals("Bullet")) {
            if (bossState != BossState.Shield) {
                BulletScript bulletScript = other.gameObject.GetComponent<BulletScript>();
                HealthPoint -= bulletScript.BulletDamage;

                bulletScript.GetComponent<Collider>().enabled = false;
                if (HealthPoint <= 0) {
                    Dead();
                    if (Achievement.bossKills < 1) {
                        Achievement.bossKills = 1;
                    }
                }
            }
            Destroy(other.gameObject);
        }
    }

    #region PauseMethod
    private void Dead() {
        GetComponent<Collider>().enabled = false;
        navMeshAgent.enabled = false;

        GameManager.Instance.UpdateScore(ScoreReward, DeathAudioClip);
        StartCoroutine(DeadAnimation());
    }

    IEnumerator DeadAnimation() {
        anim.SetInteger("Death", Random.Range(1, 3));
        yield return new WaitForSeconds(3.5f);
        Destroy(gameObject);
    }
    #endregion
}
