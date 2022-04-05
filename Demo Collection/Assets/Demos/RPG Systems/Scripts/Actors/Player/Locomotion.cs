using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Locomotion : ControllerModule
{
    [Header("Turn On/Off Features")]
    public bool enableSprinting;
    [SerializeField] private bool enableJump;
    [SerializeField] private bool enableAutoLedgeJump;

    [Header("Movement Parameters")]
    [Tooltip("Adjust the speed the player moves at.")]
    [SerializeField] private float movementSpeed;
    [Tooltip("Adjust the smoothing period at which the player rotates.")]
    [SerializeField] private float turnTime;
    [Tooltip("Adjust the speed at which the player rotates.")]
    [SerializeField] private float turnSpeed;
    [Tooltip("The force added to the rigidbody when jumping")]
    [SerializeField] private float jumpForce;
    [Tooltip("Ground check distance")]
    [SerializeField] private float distanceToGround;
    [Tooltip("How steep an angle the player can climb")]
    [SerializeField] private float maxSlopeAngle;
    [Tooltip("Time off ground in which can jump")]
    [SerializeField] private float jumpGracePeriod;

    [Header("Tracking")]
    private Vector3 lastPos;
    private float accelerateTimer;
    private bool isSprinting;
    private float slopeAngle;
    private bool isGrounded;
    private float? lastGroundedTime;
    private float? jumpButtonPressTime;
    private float airTime;
    private bool waitingToJump;
    private bool canJump;
    private bool waitingToDrop;
    private bool canDrop;

    private void Update()
    {
        switch (controller.currentState)
        {
            case Controller.characterStates.active:
                CheckSprint();
                controller.anim.SetBool("isSprinting", isSprinting);
                controller.anim.SetBool("isGrounded", isGrounded);
                controller.anim.SetFloat("AirTime", airTime);
                controller.anim.SetFloat("MoveY", controller.readInput.InputDir.magnitude);
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (controller.currentState)
        {
            case Controller.characterStates.active:
                Movement();
                break;

            case Controller.characterStates.dialogue:
                if (controller.readInput.InputDir.magnitude > 0.1f)
                {
                    controller.readInput.InputDir -= new Vector2(1, 1) * Time.deltaTime;
                }
                else
                {
                    controller.readInput.InputDir = Vector2.zero;
                }
                break;
        }
    }

    private void Movement()
    {
        float moveX = 0;
        float moveY = 0;
        moveX = controller.readInput.InputDir.x * Mathf.Lerp(0, 1, accelerateTimer);
        moveY = controller.readInput.InputDir.y * Mathf.Lerp(0, 1, accelerateTimer);

        Vector3 currentYVelocity = new Vector3(0, controller.rb.velocity.y, 0);
        Vector3 direction = new Vector3(moveX, 0f, moveY).normalized;
        Vector3 moveDir = Vector3.zero;

        if (direction.magnitude > 0)
        {
            moveDir = Turn(direction);
        }

        isGrounded = GroundCheck();

        //Requires fixing
        float sprintBoost = 1f;
        if (!isSprinting)
        {
            DropFromLedge();
        }
        else
        {
            if (enableAutoLedgeJump)
            {
                JumpFromLedge();
            }
            sprintBoost = 1.5f;
        }

        if (!enableAutoLedgeJump)
        {
            if (Time.time - lastGroundedTime <= jumpGracePeriod)
            {
                float slopeModifier = SlopeModifier(moveY);
                controller.rb.velocity = currentYVelocity + moveDir * movementSpeed * slopeModifier;
            }
            else if (!isGrounded && airTime > 0.5f)
            {
                controller.rb.velocity = currentYVelocity + moveDir * movementSpeed;
            }
        }
        else
        {
            if(isGrounded)
            {
                float slopeModifier = SlopeModifier(moveY);
                controller.rb.velocity = currentYVelocity + moveDir * movementSpeed * slopeModifier;
            }
        }

        if (enableJump && controller.readInput.jumpHeld)
        {
            OnJump();
        }

        if (isGrounded)
        {
            controller.rb.velocity = currentYVelocity + moveDir * movementSpeed * sprintBoost;
        }

        Acceleration();
    }

    private Vector3 Turn(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSpeed, turnTime);
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
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, distanceToGround + 0.1f, controller.GetGroundLayer()))
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

    private void JumpFromLedge()
    {
        if (isGrounded && canJump)
        {
            bool foundGround = CheckAheadForGround();

            if (!foundGround)
            {
                LedgeJump();
            }
        }
    }

    private void DropFromLedge()
    {
        if (!isGrounded || !canDrop) return;

        bool foundGround = CheckAheadForGround();

        if (!foundGround)
        {
            OnDrop();
        }
    }

    private void LedgeJump()
    {
        controller.rb.velocity = new Vector3(controller.rb.velocity.x, 0f, controller.rb.velocity.z) + transform.up * jumpForce;
        controller.anim.SetTrigger("Jump");
        isGrounded = false;
        canJump = false;
    }

    private void OnJump()
    {
        jumpButtonPressTime = Time.time;

        if (canJump && Time.time - jumpButtonPressTime <= jumpGracePeriod && Time.time - lastGroundedTime <= jumpGracePeriod)
        {
            Debug.Log("o");
            controller.rb.velocity = new Vector3(controller.rb.velocity.x, 0f, controller.rb.velocity.z) + transform.up * jumpForce;
            controller.anim.SetTrigger("Jump");
            isGrounded = false;
            canJump = false;
            jumpButtonPressTime = null;
            lastGroundedTime = null;
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
        if (Physics.Raycast(transform.position + transform.up * (0.5f) + (transform.forward * 0.2f), Vector3.down, out hit, 0.75f, controller.GetGroundLayer()))
        {
            return (hit.collider);
        }
        return false;
    }

    private void CheckSprint()
    {
        isSprinting = controller.readInput.sprintHeld;
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
