using System.Collections;
using System.Collections.Generic;
using Luminosity.IO.Examples;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour {
    public bool isSlowed = false;
    private const float slowSpeedTime = 5;
    private float slowLeft = 0;

    [Tooltip("Speed of the enemy")]
    public float MovementSpeed;

    [Tooltip("Damage on player on touch")]
    public int ContactDamage;

    [Tooltip("Health of the enemy")]
    public int HealthPoint;

    [Tooltip("Score reward for destorying enemy")]
    public int ScoreReward;

    [Tooltip("Sound upon death")]
    public AudioClip DeathAudioClip;

    private Transform playerTransform;
    private NavMeshAgent navMeshAgent;
    private Animator anim;

    private bool isAlive = true;
    
    void Start() {
        playerTransform = GameObject.Find("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        navMeshAgent.speed = MovementSpeed;
    }
    
    void Update() {
        if (isSlowed) {
            slowLeft += Time.deltaTime;
            MovementSpeed = 0.5f;
            navMeshAgent.speed = MovementSpeed;

            if (slowLeft > slowSpeedTime) {
                isSlowed = false;
                slowLeft = 0;
            }
        }

        if (playerTransform != null && !GameManager.Instance.isGameOver && isAlive) {
            navMeshAgent.SetDestination(playerTransform.position);
            anim.SetBool("Run", true);
        }

        if (GameManager.Instance.isGameOver && navMeshAgent.enabled == true) {
            navMeshAgent.enabled = false;
            anim.SetInteger("Victory", Random.Range(1, 3));
        }

        if (GameManager.isPaused) {
            MovementSpeed = 0;
            navMeshAgent.speed = MovementSpeed;
        }
        else if (!GameManager.isPaused) {
            MovementSpeed = 1;
            navMeshAgent.speed = MovementSpeed;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag.Equals("Bullet")) {
            BulletScript bulletScript = other.gameObject.GetComponent<BulletScript>();

            switch (bulletScript.ReturnBulletType()) {
                case BulletScript.BulletType.Minigun:
                    HealthPoint -= bulletScript.BulletDamage;
                    break;
                case BulletScript.BulletType.Normal:
                    HealthPoint -= bulletScript.BulletDamage;
                    break;
            }

            Destroy(bulletScript.gameObject);
            bulletScript.GetComponent<Collider>().enabled = false;

            if (HealthPoint <= 0) {
                if (Achievement.minionKills < 10)
                    Achievement.minionKills++;

                Dead();
            }
        }
    }

    private void Dead() {

        isAlive = false;

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
}
