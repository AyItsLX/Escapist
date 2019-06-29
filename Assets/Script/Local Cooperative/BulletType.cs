using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletType : MonoBehaviour {
    
    public float MovementSpeed;

    public AudioClip ImpactAudioOnWall;
    public AudioClip[] ImpactAudioOnEnemy;

    [SerializeField]
    public int BulletDamage;

    private Rigidbody rb;
    private AudioSource audioSource;
    private float timeToLive = 3;

    void Start() {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        rb.velocity = transform.forward * MovementSpeed;
        Destroy(gameObject, timeToLive);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag.Equals("Enemy")) {
            audioSource.PlayOneShot(ImpactAudioOnEnemy[Random.Range(0, ImpactAudioOnEnemy.Length)], 1);
            rb.useGravity = true;
        }
        else if (collision.gameObject.tag.Equals("Wall")) {
            audioSource.PlayOneShot(ImpactAudioOnWall, .25f);
            GetComponent<BulletType>().enabled = false;
            rb.useGravity = true;
        }
    }
}