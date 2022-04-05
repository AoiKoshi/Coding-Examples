using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    //Generic rotate script on the z-axis.
    [SerializeField]
    private float speed = 0.2f;

    void Update()
    {
        gameObject.transform.Rotate(0f, 0f, -speed);
    }
}
