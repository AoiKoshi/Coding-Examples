using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoneTrigger : MonoBehaviour
{
    [SerializeField]
    GameObject _camToTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Camera.main.gameObject.SetActive(false);
            _camToTrigger.SetActive(true);
        }
    }
}
