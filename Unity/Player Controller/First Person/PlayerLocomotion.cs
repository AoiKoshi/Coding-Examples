using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    public SceneManagement SM;

    public enum playerState
    {
        idle,
        walking,
        sprinting
    }

    public enum playerState2
    {
        standing,
        crouching
    }

    public playerState moveState = playerState.idle;
    public playerState2 crouchState = playerState2.standing;
    [SerializedField] private Camera eyes;
    [SerializedField] private float moveSpeed;
    [SerializedField] private float turnSpeed;
    [SerializedField] private float jumpHeight;

    private Rigidbody rb;
    private bool isGrounded;
    private float pitch;
    private float yaw;
    private float speedMultiplier = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        isGrounded = true;
    }

    private void Update()
    {
        if (SM.gameState == SceneManagement.playState.running)
        {
            EyesInput();
            isGrounded = GroundCheck();
            Movement();
            IdleCheck();
        }
    }

    private void EyesInput()
    {
        pitch += (-Input.GetAxisRaw("Mouse Y") + (-Input.GetAxis("LookVertical")) * 3) * -100f * Time.deltaTime;
        yaw = (Input.GetAxisRaw("Mouse X") + Input.GetAxis("LookHorizontal") * 3) * 100f * Time.deltaTime;

        if (pitch < -70)
        {
            pitch = -70;
        }
        else if (pitch > 100)
        {
            pitch = 100;
        }

        eyes.transform.localRotation = Quaternion.AngleAxis(-pitch, Vector3.right);
        this.transform.Rotate(0, yaw, 0);
    }

    private bool GroundCheck()
    {
        //Debug.DrawRay(transform.position, -transform.up * 1.1f, Color.red);
        if (Physics.Raycast(transform.position, -transform.up, 1.1f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Movement()
    {
        if (Input.GetButton("Sprint") && isGrounded)
        {
            speedMultiplier = 1.5f;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speedMultiplier = 1f;
        }

        float moveHorizontal = Input.GetAxis("Horizontal") * speedMultiplier;
        float moveVertical = Input.GetAxis("Vertical") * speedMultiplier;
        rb.velocity = new Vector3(0, rb.velocity.y, 0) + (transform.forward * moveSpeed * moveVertical) + (eyes.transform.right * moveSpeed * moveHorizontal);
        
        if (isGrounded && Input.GetButtonDown("Jump") && crouchState == playerState2.standing)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z) + transform.up * jumpHeight;
        }
        else if (isGrounded && Input.GetButtonDown("Crouch") && crouchState == playerState2.standing)
        {
            crouchState = playerState2.crouching;
        }
        else if (isGrounded && (Input.GetButtonDown("Jump") || Input.GetButtonDown("Crouch")) && crouchState == playerState2.crouching)
        {
            crouchState = playerState2.standing;
        }
    }

    private void IdleCheck()
    {
        if (isGrounded && rb.velocity == new Vector3(0, rb.velocity.y, 0))
        {
            moveState = playerState.idle;
        }
        else if (speedMultiplier == 1.5f)
        {
            moveState = playerState.sprinting;
        }
        else
        {
            moveState = playerState.walking;
        }
    }
}