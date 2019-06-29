using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLight : MonoBehaviour {

    public bool hasSpotlight = false;

    public List<GameObject> lights;

    private AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            audioSource.Play();

            GetComponent<Collider>().enabled = false;

            if (hasSpotlight) {
                transform.GetChild(0).gameObject.SetActive(false);
            }

            foreach (GameObject lightObject in lights) {
                lightObject.SetActive(true);
            }
        }
    }
}
