using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [Header("Networked Variables")]
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector2> MovementInput = new NetworkVariable<Vector2>();

    [Header("Client-side Variables")]
    public Animator Anim;
    public Rigidbody RBody;
    public Camera MainCam;
    [Tooltip("Attach the free look Cinemachine component.")]
    public CinemachineFreeLook freelookCam;

    [Header("Modules")]
    public ReadInput ReadInputModule;
    public Locomotion LocomotionModule;

    [Header("Camera Transforms")]
    [SerializeField] private Transform hips;
    [SerializeField] private Transform head;

    void Start()
    {
        RBody = GetComponent<Rigidbody>();
        RBody.freezeRotation = true;
        MainCam = Camera.main;

        if (IsOwner)
        {
            CameraAssigner.Instance.RegisterPlayer(transform, hips, head);
        }
        else if (!IsOwner)
        {
            ReadInputModule.enabled = false;
            LocomotionModule.enabled = false;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            OnMoveDirection();
        }
        else
        {
            Anim.SetFloat("MoveY", MovementInput.Value.magnitude);
        }
    }

    public void OnMoveDirection()
    {
        MovementInput = new NetworkVariable<Vector2>(ReadInputModule.InputDir);
        Anim.SetFloat("MoveY", MovementInput.Value.magnitude);
    }
}
