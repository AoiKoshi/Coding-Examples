using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatInSpace : MonoBehaviour
{
    [SerializeField]
    private float degreesPerSecond;
    [SerializeField]
    private float maxDegrees;
    [SerializeField]
    private float minDegrees;
    [SerializeField]
    private float amplitude;
    [SerializeField]
    private float frequency;

    private bool isRotatingRight = true;

    [SerializeField]
    private Vector3 originalPos;
    [SerializeField]
    private Vector3 tempPos;

    void Start()
    {
        originalPos = transform.position;
    }

    void Update()
    {
        if(isRotatingRight)
        {
            RotatingRight();
        }
        else
        {
            RotatingLeft();
        }
        
        tempPos = originalPos;
        tempPos.y += Mathf.Sin(Time.time * Mathf.PI * frequency) * amplitude;
        transform.position = tempPos;
    }

    private void RotatingRight()
    {
        transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
        if (transform.rotation.eulerAngles.y > maxDegrees && transform.rotation.eulerAngles.y < maxDegrees + 1)
        {
            isRotatingRight = false;
        }
    }

    private void RotatingLeft()
    {
        transform.Rotate(new Vector3(0f, -Time.deltaTime * degreesPerSecond, 0f), Space.World);
        if (transform.rotation.eulerAngles.y < minDegrees && transform.rotation.eulerAngles.y > minDegrees - 1)
        {
            isRotatingRight = true;
        }
    }
}
