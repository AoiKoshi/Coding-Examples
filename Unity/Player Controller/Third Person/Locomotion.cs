using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Locomotion : ControllerModule
{
    [Header("Turn On/Off Features")]
    [SerializeField]
    private bool enableJump;
    [SerializeField]
    private bool enableAutoSprintJump;
    public bool enableSprinting;

    [Header("Tracking")]
    private Vector3 lastPos;
    private float accelerateTimer;
    private bool isGrounded;
    private float airTime;
    [HideInInspector]
    public bool isSprinting;
    private bool waitingToJump;
    private bool canJump;
    private bool waitingToDrop;
    private bool canDrop;

    private void FixedUpdate()
    {
        if (controller.currentState == Controller.characterStates.dialogue)
        {
            if (controller.currentMove.magnitude > 0.1f)
            {
                controller.currentMove -= new Vector2(1, 1) * Time.deltaTime;
            }
            else
            {
                controller.currentMove = Vector2.zero;
            }
        }
        else if (controller.currentState == Controller.characterStates.active)
        {
            Movement();
        }
    }

    private void Movement()
    {
        float moveX = 0;
        float moveY = 0;
        moveX = controller.currentMove.x * Mathf.Lerp(0, 1, accelerateTimer);
        moveY = controller.currentMove.y * Mathf.Lerp(0, 1, accelerateTimer);

        Vector3 currentYVelocity = new Vector3(0, controller.rb.velocity.y, 0);
        Vector3 direction = new Vector3(moveX, 0f, moveY).normalized;
        Vector3 moveDir = Vector3.zero;

        if (direction.magnitude > 0)
        {
            moveDir = Turning(direction);
        }

        isGrounded = GroundCheck();

        float sprintBoost = 1f;
        if (isSprinting)
        {
            if (enableAutoSprintJump)
            {
                JumpFromLedge();
            }
            else
            {
                DropFromLedge();
            }
            sprintBoost = 1.5f;
        }
        else if (!isSprinting)
        {
            DropFromLedge();
        }

        if (isGrounded)
        {
            controller.rb.velocity = currentYVelocity + moveDir * controller.attributes.movementSpeed * sprintBoost;
            controller.anim.SetFloat("MoveY", controller.currentMove.magnitude);
        }

        controller.anim.SetBool("isSprinting", isSprinting);
        controller.anim.SetBool("isGrounded", isGrounded);
        Acceleration();
    }

    private Vector3 Turning(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref controller.attributes.turnSpeed, controller.attributes.turnTime);
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        return moveDir;
    }

    private void Acceleration()
    {
        if (controller.rb.velocity != Vector3.zero && accelerateTimer <= 1f)
        {
            accelerateTimer += Time.fixedDeltaTime * 2f;
        }
        else if (controller.rb.velocity == Vector3.zero)
        {
            accelerateTimer = 0.1f;
        }
    }

    private bool GroundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up * (0.5f), Vector3.down, out hit, 0.75f, controller.groundLayer))
        {
            if (hit.collider)
            {
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
                return true;
            }
        }
        airTime += Time.deltaTime;
        controller.anim.SetFloat("AirTime", airTime);
        return false;
    }
    private void JumpFromLedge()
    {
        if (isGrounded && canJump)
        {
            bool foundGround = CheckAheadForGround();

            if (!foundGround)
            {
                OnLedgeJump();
            }
        }
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

    private void OnLedgeJump()
    {
        controller.rb.velocity = new Vector3(controller.rb.velocity.x, 0f, controller.rb.velocity.z) + transform.up * controller.attributes.jumpForce;
        controller.anim.SetTrigger("Jump");
        isGrounded = false;
        canJump = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (enableJump && GroundCheck())
        {
            if (context.phase == InputActionPhase.Started)
            {
                controller.rb.velocity = new Vector3(controller.rb.velocity.x, 0f, controller.rb.velocity.z) + transform.up * controller.attributes.jumpForce;
                controller.anim.SetTrigger("Jump");
                isGrounded = false;
                canJump = false;
            }
        }
    }

    private void OnDrop()
    {
        controller.rb.velocity = new Vector3(controller.rb.velocity.x, 0f, controller.rb.velocity.z) + transform.forward * 0.5f;
        controller.anim.SetTrigger("Drop");
        isGrounded = false;
        canDrop = false;
    }

    private bool CheckAheadForGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up * (0.5f) + (transform.forward * 0.2f), Vector3.down, out hit, 0.75f, controller.groundLayer))
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
