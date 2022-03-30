using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller))]
public class ControllerModule : MonoBehaviour
{
    [HideInInspector]
    public Controller controller;

    private void Awake()
    {
        controller = GetComponent<Controller>();
    }
}
