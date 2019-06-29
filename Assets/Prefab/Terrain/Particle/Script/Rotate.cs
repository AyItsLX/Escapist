using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
    public float speed = 2f;
    public float maxRotation = 45f;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler(maxRotation * Mathf.Sin(Time.time * speed), maxRotation * Mathf.Sin(Time.time * speed), maxRotation * Mathf.Sin(Time.time * speed));

    }
}
