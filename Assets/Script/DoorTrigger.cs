using System.Collections;
using System.Collections.Generic;
using Luminosity.IO.Examples;
using UnityEngine;

public class DoorTrigger : MonoBehaviour {
    public bool isTriggered;

    //private ActivatableDoor activatableDoor;
    void Start ()
    {
        isTriggered = false;
    }
    //public void SetActivatableDoor(ActivatableDoor _activatableDoor)
    //{
    //    activatableDoor = _activatableDoor;
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            GameManager.Instance.keyNeeded--;
            GameManager.Instance.keyObj[GameManager.Instance.keyNeeded].SetActive(false);
            isTriggered = true;
            gameObject.SetActive(false);
        }
    }
}
