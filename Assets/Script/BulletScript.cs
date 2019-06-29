using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    public enum BulletType { Minigun, Rocket, Ice, Normal };
    public BulletType bulletType;

    [Tooltip("Speed of the bullet")]
    public float MovementSpeed;

    [Tooltip("Sound upon impact")]
    public AudioClip ImpactAudioOnWall;
    public AudioClip[] ImpactAudioOnEnemy;

    [SerializeField]
    public int BulletDamage;

    private Rigidbody rb;
    private AudioSource audioSource;
    private float timeToLive = 3;

    public LayerMask layerMask;
    void Start() {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        rb.velocity = transform.forward * MovementSpeed;
        Destroy(gameObject, timeToLive);
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.tag.Equals("Enemy")) {
            audioSource.PlayOneShot(ImpactAudioOnEnemy[Random.Range(0, ImpactAudioOnEnemy.Length)], 1);
            //switch (bulletType) {
            //    case BulletType.Rocket:
            //        print("Exploded");
            //        Explosive();
            //        break;
            //    case BulletType.Ice:
            //        print("Iced");
            //        Ice();
            //        break;
            //}
            //Destroy(gameObject);
        }
        else if (collision.gameObject.tag.Equals("Wall")) {
            //switch (bulletType) {
            //    case BulletType.Rocket:
            //        print("Exploded");
            //        Explosive();
            //        break;
            //    case BulletType.Ice:
            //        print("Iced");
            //        Ice();
            //        break;
            //}
            //Destroy(gameObject);
            audioSource.PlayOneShot(ImpactAudioOnWall, .25f);
            GetComponent<BulletScript>().enabled = false;
        }

        rb.useGravity = true;
    }

    private void Explosive() {
        Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, 50, layerMask);

        foreach (Collider nearby in nearbyTargets) {
            EnemyScript enemyScript = nearby.GetComponent<EnemyScript>();
            Rigidbody rb = nearby.GetComponent<Rigidbody>();

            rb.AddExplosionForce(700f, transform.position, 10);
            enemyScript.HealthPoint -= 100;
        }
    }

    private void Ice() {
        Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, 50, layerMask);

        foreach (Collider nearby in nearbyTargets) {
            EnemyScript enemyScript = nearby.GetComponent<EnemyScript>();
            enemyScript.isSlowed = true;
        }
    }

    public BulletType ReturnBulletType() {
        return bulletType;
    }
}