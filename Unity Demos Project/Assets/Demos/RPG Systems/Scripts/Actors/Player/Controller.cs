using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(Locomotion))]
[RequireComponent(typeof(ReadInput))]
public class Controller : MonoBehaviour
{
    public static Controller Instance;

    [Header("Components")]
    public Animator anim;
    public Rigidbody rb;
    [SerializeField] protected Camera mainCam;
    [SerializeField] [Tooltip("Attach the free look Cinemachine component.")]
    protected CinemachineFreeLook freeLookCam;

    [Header("Layer Masks")]
    [SerializeField] protected LayerMask groundLayer;

    [Header("Modules")]
    public Locomotion locomotion;
    public ReadInput readInput;

    [Header("States")]
    public characterStates currentState;
    public enum characterStates
    {
        active,
        vaulting,
        dialogue,
        paused
    }

    private void Awake() => Instance = this;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        locomotion = GetComponent<Locomotion>();
        readInput = GetComponent<ReadInput>();
    }

    public virtual void OnAction() { }

    public LayerMask GetGroundLayer()
    {
        return groundLayer;
    }
}
