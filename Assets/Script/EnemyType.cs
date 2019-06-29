using System.Collections;
using System.Collections.Generic;
using Luminosity.IO.Examples;
using UnityEngine;
using UnityEngine.AI;

public class EnemyType : MonoBehaviour {

    public enum EnemyClass { Melee, Ranged };
    public EnemyClass enemyClass;

    [Header("Stat")]
    public float enemyHealth;
    public int enemyDamage;
    public int enemyAttackSpeed;
    public int enemyScore;
    public float enemyMovementSpeed;

    public bool foundTarget = false;
    public float timeBeforeRelease = 1;

    public AudioClip DeathAudioClip;

    private GameObject objectiveObj;
    private Transform playerTransform;
    private NavMeshAgent navMeshAgent;
    private Animator anim;

    public float closestDistanceSqr = 2;
    public float collisionRadius = 2;

    public Collider[] nearbyTargets;
    public Transform closestTarget;
    public LayerMask layerMask;

    void Start () {
        objectiveObj = GameObject.Find("Objective");
        playerTransform = GameObject.Find("PlayerOne").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        switch (enemyClass) {
            case EnemyClass.Melee:
                MeleeAttribute meleeType = new MeleeAttribute(10, 100, 1, 5, .5f);
                SetMeleeStat(meleeType.damage, meleeType.health, meleeType.attackSpeed, meleeType.score, meleeType.movementSpeed);
                navMeshAgent.speed = enemyMovementSpeed;
                enemyHealth = GUIManager.increasedEnemyHealth;
                break;
            //case EnemyClass.Ranged:
            //    RangedAttribute rangeType = new RangedAttribute(3, 10, 100, 10, 1, 1);
            //    navMeshAgent.speed = rangeType.movementSpeed;
            //    break;
        }

        if (objectiveObj != null)
            navMeshAgent.SetDestination(objectiveObj.transform.position);
    }
	
	void Update () {
        #region 1
        //if (playerTransform != null && !GameManager.Instance.isGameOver && isAlive) {
        //    navMeshAgent.SetDestination(playerTransform.position);
        //    anim.SetBool("Run", true);
        //}

        //if (GameManager.Instance.isGameOver && navMeshAgent.enabled == true) {
        //navMeshAgent.enabled = false;
        //anim.SetInteger("Victory", Random.Range(1, 3));
        //}

        //if (GameManager.isPaused) {
        //    MovementSpeed = 0;
        //    navMeshAgent.speed = MovementSpeed;
        //}
        //else if (!GameManager.isPaused) {
        //    MovementSpeed = 1;
        //    navMeshAgent.speed = MovementSpeed;
        //}
        #endregion
        switch (enemyClass) {
            case EnemyClass.Melee:
                navMeshAgent.speed = enemyMovementSpeed;
                CheckRange();
                break;
            //case EnemyClass.Ranged:
            //    RangedAttribute rangeType = new RangedAttribute(3, 10, 100, 10, 1, 1);
            //    navMeshAgent.speed = rangeType.movementSpeed;
            //    CheckFOV();
            //    break;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.name == "Objective") {
            collision.gameObject.GetComponent<ObjectiveHealth>().HitObjective(100);
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag.Equals("Bullet")) {
            BulletType bulletType = collision.gameObject.GetComponent<BulletType>();

            enemyHealth -= bulletType.BulletDamage;

            bulletType.GetComponent<Collider>().enabled = false;
            if (enemyHealth <= 0) {
                GUIManager.GUIManager_Instance.goldCount += 25;
                GUIManager.GUIManager_Instance.experienceCount += 25;
                GUIManager.GUIManager_Instance.UpdateExperience();
                GUIManager.GUIManager_Instance.UpdateGold();
                Destroy(gameObject);
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, collisionRadius);
    }

    void CheckRange() {
        nearbyTargets = Physics.OverlapSphere(transform.position, collisionRadius, layerMask);
        int i = 0;
        while (i < nearbyTargets.Length) {
            i++;
        }
        if (i > 0) {
            foreach (Collider target in nearbyTargets) {
                Vector3 directionToTarget = target.transform.position - transform.position;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr) {
                    closestDistanceSqr = dSqrToTarget;
                    closestTarget = target.transform;
                    foundTarget = true;
                }
            }
        }
        if (closestTarget != null) {
            print("Walking to the player");
            navMeshAgent.SetDestination(closestTarget.position);
        }

        if (foundTarget) {
            timeBeforeRelease -= Time.deltaTime;
            if (timeBeforeRelease <= 0) {
                foundTarget = false;
                timeBeforeRelease = 1;
                closestDistanceSqr = 2;
                closestTarget = objectiveObj.transform;
            }
        }
    }

    void CheckFOV() {
        //Vector3 newPosition = new Vector3(transform.position.x, playerTransform.position.y, transform.position.z);
        Vector3 dirVec = playerTransform.position - transform.position;
        //print(dirVec);
        float angle = Vector3.Angle(dirVec, transform.forward);

        if (dirVec.magnitude < 5 && angle < 45) {
            print("Facing the player");
            transform.forward = Vector3.Lerp(transform.forward, dirVec, Time.deltaTime);

            //meshRenderer.material = redMaterial;

            //if (!isPlayerDetected) {
            //isPlayerDetected = true;
            //audioSource.clip = audioClipList[0];
            //audioSource.Play();
            //}
        }
        else {
            //meshRenderer.material = cyanMaterial;
            //isPlayerDetected = false;
        }
    }

    public void SetEnemyType(EnemyClass enemyClass) {
        this.enemyClass = enemyClass;
    }

    public void SetMeleeStat(int Damage, int Health, int AttackSpeed, int Score, float MovementSpeed) {
        enemyDamage = Damage;
        enemyHealth = Health;
        enemyAttackSpeed = AttackSpeed;
        enemyScore = Score;
        enemyMovementSpeed = MovementSpeed;
    }

    struct RangedAttribute {
        public int range;
        public int damage;
        public int health;
        public int attackSpeed;
        public int score;
        public float movementSpeed;
        public RangedAttribute(int Range, int Damage, int Health, int AttackSpeed, int Score, float MovementSpeed) {
            range = Range;
            damage = Damage;
            health = Health;
            attackSpeed = AttackSpeed;
            score = Score;
            movementSpeed = MovementSpeed;
        }
    }

    struct MeleeAttribute {
        public int damage;
        public int health;
        public int attackSpeed;
        public int score;
        public float movementSpeed;
        public MeleeAttribute(int Damage, int Health, int AttackSpeed, int Score, float MovementSpeed) {
            damage = Damage;
            health = Health;
            attackSpeed = AttackSpeed;
            score = Score;
            movementSpeed = MovementSpeed;
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag.Equals("Bullet")) {
    //        BulletScript bulletScript = collision.gameObject.GetComponent<BulletScript>();
    //        HealthPoint -= bulletScript.BulletDamage;

    //        //Destroy(bulletScript.gameObject);
    //        bulletScript.GetComponent<Collider>().enabled = false;

    //        if (HealthPoint <= 0) {
    //            Dead();
    //        }
    //    }
    //}

    //private void Dead() {

    //    isAlive = false;

    //    GetComponent<Collider>().enabled = false;
    //    navMeshAgent.enabled = false;

    //    //GameManager.Instance.UpdateScore(ScoreReward, DeathAudioClip);
    //    StartCoroutine(DeadAnimation());
    //}

    //IEnumerator DeadAnimation() {
    //    anim.SetInteger("Death", Random.Range(1, 3));
    //    yield return new WaitForSeconds(3.5f);
    //    Destroy(gameObject);
    //}

    // For Reference, creating a object in C3
    //InventorySlot slot1 = new InventorySlot(false, "Slot 1");

    //struct InventorySlot {
    //    //Variable declaration
    //    //Note: I'm explicitly declaring them as public, but they are public by default. You can use private if you choose.
    //    public bool IsFree;
    //    public string Name;

    //    //Constructor (not necessary, but helpful)
    //    public InventorySlot(bool isFree, string name) {
    //        this.IsFree = isFree;
    //        this.Name = name;
    //    }
    //}
}