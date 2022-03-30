using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System;

[RequireComponent(typeof(PlayerController))]
public class Locomotion : NetworkBehaviour
{
    public Vector3 Position = new Vector3();

    [SerializeField] private LayerMask groundMask;
    private PlayerController Controller;
    private ReadInput Inputs;
    private Rigidbody RBody;
    private Camera MainCam;
    private Animator Anim;

    [Header("Properties")]
    [Tooltip("Adjust the speed the player moves at.")]
    [SerializeField] private float movementSpeed;
    [Tooltip("Adjust the smoothing period at which the player rotates.")]
    [SerializeField] private float turnTime;
    [Tooltip("Adjust the speed at which the player rotates.")]
    [SerializeField] private float turnSpeed;
    [Tooltip("The force added to the rigidbody when jumping")]
    [SerializeField] private float jumpForce;

    [Header("Turn On/Off Features")]
    [SerializeField] private bool enableJump;

    [Header("Movement Parameters")]
    [SerializeField] private float distanceToGround;
    [Tooltip("How steep an angle the player can climb")]
    [SerializeField] private float maxSlopeAngle;
    [Tooltip("Time off ground in which can jump")]
    [SerializeField] private float jumpGracePeriod;

    [Header("Tracking")]
    private Vector3 lastPos;
    private Vector3 lastVel;
    private float accelerateTimer;
    private float slopeAngle;
    private bool isGrounded;
    private float? lastGroundedTime;
    private float? jumpButtonPressTime;
    private bool waitingToJump;
    private bool canJump;
    private bool waitingToDrop;
    private bool canDrop;
    private float airTime;

    void Start()
    {
        Controller = GetComponent<PlayerController>();
        Inputs = Controller.ReadInputModule;
        RBody = Controller.RBody;
        MainCam = Controller.MainCam;
        Anim = Controller.Anim;
    }

    void FixedUpdate()
    {
        Movement();
        SetPos();
        MovePos();
        Anim.SetBool("isGrounded", isGrounded);
        TriggerAnimBoolClientRpc("isGrounded", isGrounded);
        Anim.SetFloat("AirTime", airTime);
        TriggerAnimFloatClientRpc("AirTime", airTime);
    }

    [ClientRpc]
    private void TriggerAnimBoolClientRpc(string name, bool value)
    {
        if (!IsLocalPlayer)
        {
            Anim.SetBool(name, value);
        }
    }

    [ClientRpc]
    private void TriggerAnimFloatClientRpc(string name, float value)
    {
        if (!IsLocalPlayer)
        {
            Anim.SetFloat(name, value);
        }
    }

    private void SetPos()
    {
        Position = transform.position + (lastVel * Time.deltaTime);
    }

    private void MovePos()
    {
        transform.position = Position;
    }

    private void Movement()
    {
        Vector3 velocity = new Vector3();

        float moveX = 0;
        float moveY = 0;
        moveX = Inputs.InputDir.x * Mathf.Lerp(0f, 1f, accelerateTimer);
        moveY = Inputs.InputDir.y * Mathf.Lerp(0f, 1f, accelerateTimer);

        Vector3 currentYVelocity = new Vector3(0, RBody.velocity.y, 0);
        Vector3 direction = new Vector3(moveX, 0f, moveY).normalized;
        Vector3 moveDir = Vector3.zero;

        if (direction.magnitude > 0)
        {
            moveDir = Turn(direction);
        }

        isGrounded = GroundCheck();

        DropFromLedge();

        if (Time.time - lastGroundedTime <= jumpGracePeriod)
        {
            float slopeModifier = SlopeModifier(moveY);
            velocity = currentYVelocity + moveDir * movementSpeed * slopeModifier;
        }
        else if (!isGrounded && airTime > 0.5f)
        {
            velocity = currentYVelocity + moveDir * movementSpeed;
        }

        Acceleration();
        lastVel = velocity;
    }

    private Vector3 Turn(Vector3 direction)
    {
        var camDir = Inputs.CameraDir;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camDir.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSpeed, turnTime);
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        return moveDir;
    }

    private void Acceleration()
    {
        if (RBody.velocity != Vector3.zero && accelerateTimer <= 1f)
        {
            accelerateTimer += Time.fixedDeltaTime * 2f;
        }
        else if (RBody.velocity == Vector3.zero)
        {
            accelerateTimer = 0.1f;
        }
    }

    private bool GroundCheck()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up;

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, distanceToGround + 0.1f, groundMask))
        {
            if (hit.collider)
            {
                lastGroundedTime = Time.time;
                if (!canJump && !waitingToJump)
                {
                    waitingToJump = true;
                    StartCoroutine(DelayCanJump(0.5f));
                }

                if (!canDrop && !waitingToDrop)
                {
                    waitingToDrop = true;
                    StartCoroutine(DelayCanDrop(0.5f));
                }

                slopeAngle = Vector3.Angle(hit.normal, transform.forward) - 90f;
                return true;
            }
        }

        airTime += Time.deltaTime;
        return false;
    }

    private float SlopeModifier(float moveY)
    {
        float speed = 1f;

        if (moveY > 0f)
        {
            speed = 1f - (slopeAngle / maxSlopeAngle);
        }

        else if (moveY < 0f)
        {
            if (slopeAngle < 0f)
            {
                speed = 1f - (Mathf.Abs(slopeAngle) / maxSlopeAngle);
            }
        }

        return speed;
    }

    private void DropFromLedge()
    {
        if (isGrounded && canDrop)
        {
            bool foundGround = CheckAheadForGround();
            if (!foundGround)
            {
                OnDrop();
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (enableJump)
        {
            if (context.phase == InputActionPhase.Started)
            {
                jumpButtonPressTime = Time.time;

                if (canJump && Time.time - jumpButtonPressTime <= jumpGracePeriod && Time.time - lastGroundedTime <= jumpGracePeriod)
                {
                    RBody.velocity = new Vector3(RBody.velocity.x, 0f, RBody.velocity.z) + transform.up * jumpForce;
                    Controller.Anim.SetTrigger("Jump");
                    isGrounded = false;
                    canJump = false;
                    jumpButtonPressTime = null;
                    lastGroundedTime = null;
                }
            }
        }
    }

    private void OnDrop()
    {
        RBody.velocity = new Vector3(RBody.velocity.x, 0f, RBody.velocity.z) + transform.forward * 0.5f;
        Controller.Anim.SetTrigger("Drop");
        isGrounded = false;
        canDrop = false;
    }

    private bool CheckAheadForGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up * (0.5f) + (transform.forward * 0.2f), Vector3.down, out hit, 0.75f, groundMask))
        {
            if (hit.collider)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private IEnumerator DelayCanJump(float delay)
    {
        airTime = 0;
        yield return new WaitForSeconds(delay);
        canJump = true;
        waitingToJump = false;
    }

    private IEnumerator DelayCanDrop(float delay)
    {
        airTime = 0;
        yield return new WaitForSeconds(delay);
        canDrop = true;
        waitingToDrop = false;
    }
}
