using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class OverworldController : MonoBehaviour
{
    [Serializable]
    public class CharacterComponents
    {
        public Camera mainCam;
        [Tooltip("Attach the free look Cinemachine component.")]
        public CinemachineFreeLook freeLookCam;
        public Animator anim;
        [HideInInspector] public Rigidbody rb;
        public LayerMask groundLayerMask;
    }
    [SerializeField]
    private CharacterComponents components = new CharacterComponents();

    [Serializable]
    public class CharacterInputs
    {
        public PlayerInput playerInputs;
        public InputActionAsset controls;
    }
    [SerializeField]
    private CharacterInputs inputs = new CharacterInputs();

    [Serializable]
    public class CharacterProperties
    {
        [Tooltip("Adjust the speed the player moves at.")] 
        public float movementSpeed;
        [Tooltip("Adjust the smoothing period at which the player rotates.")] 
        public float turnTime;
        [Tooltip("Adjust the speed at which the player rotates.")]
        public float turnSpeed;
        [Tooltip("Adjust IK foot positioning.")]
        [Range(0, 1)]
        public float footDistanceToGround;
        public float jumpForce;
    }
    [SerializeField]
    private CharacterProperties properties = new CharacterProperties();

    [Serializable]
    public class CharacterTracking
    {
        public Vector3 lastPos;
        public float accelerateTimer;
        public bool isGrounded;
        public float airTime;
        public bool actionButtonPressed;
        public bool isSprinting;
        public bool waitingToJump;
        public bool canJump;
        public bool waitingToDrop;
        public bool canDrop;
    }
    [SerializeField]
    private CharacterTracking tracking = new CharacterTracking();

    [Serializable]
    public class CharacterParkour
    {
        public Vector3 targetPos;
        public float timer;
        [Tooltip("Adjust IK hand positioning.")]
        [Range(0, 1)]
        public float handDistanceFromObject;
        public bool canVaultOver;
        public LayerMask vaultLayer;
        public float maxVaultHeight;
        public float vaultSpeed;
        public float floatDelay;
        public Vector3 vaultPeak;
    }
    [SerializeField]
    private CharacterParkour parkour = new CharacterParkour();

    [Serializable]
    public class UI
    {
        public GameObject mainMenu;
        public GameObject itemDialogue;
        public GameObject partyDialogue;
        public GameObject pauseMenu;
    }
    [SerializeField]
    private UI userInterfaces = new UI();

    [Header("States")]
    public characterStates currentState;
    public enum characterStates
    {
        idle,
        moving,
        parkour,
        vaulting,
        dialogue,
        paused
    }

    private Vector2 currentMove;

    void Start()
    {
        components.rb = GetComponent<Rigidbody>();
        components.rb.freezeRotation = true;
        components.mainCam = Camera.main;

        parkour.canVaultOver = false;
    }

    void FixedUpdate()
    {
        if (currentState == characterStates.dialogue)
        {
            if (currentMove.magnitude > 0.1f)
            {
                currentMove -= new Vector2(1, 1) * Time.deltaTime;
            }
            else
            {
                currentMove = Vector2.zero;
            }
        }
        else if (currentState == characterStates.vaulting)
        {
            Vaulting();
        }
        else if (currentState != characterStates.paused)
        {
            HandleMove();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        currentMove = context.ReadValue<Vector2>();
    }

    private void HandleMove()
    {
        float moveX = 0;
        float moveY = 0;
        moveX = currentMove.x * Mathf.Lerp(0, 1, tracking.accelerateTimer);
        moveY = currentMove.y * Mathf.Lerp(0, 1, tracking.accelerateTimer);
        Vector3 currentYVelocity = new Vector3(0, components.rb.velocity.y, 0);
        Vector3 direction = new Vector3(moveX, 0f, moveY).normalized;
        Vector3 moveDir = Vector3.zero;

        if (direction.magnitude > 0)
        {
            currentState = characterStates.moving;

            moveDir = HandleTurning(direction);
        }

        tracking.isGrounded = GroundCheck();

        float sprintBoost = 1f;
        if (tracking.isSprinting)
        {
            Vaulting();
            JumpFromLedge();
            sprintBoost = 1.5f;
        }
        else if (!tracking.isSprinting)
        {
            DropFromLedge();
        }

        if (tracking.isGrounded)
        {
            components.rb.velocity = currentYVelocity + moveDir * properties.movementSpeed * sprintBoost;
            components.anim.SetFloat("MoveY", currentMove.magnitude);
        }
        
        components.anim.SetBool("isSprinting", tracking.isSprinting);
        components.anim.SetBool("isGrounded", tracking.isGrounded);
        HandleAcceleration();
    }

    private Vector3 HandleTurning(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + components.mainCam.transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref properties.turnSpeed, properties.turnTime);
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        return moveDir;
    }

    private void HandleAcceleration()
    {
        if (components.rb.velocity != Vector3.zero && tracking.accelerateTimer <= 1f)
        {
            tracking.accelerateTimer += Time.fixedDeltaTime * 2f;
        }
        else if(components.rb.velocity == Vector3.zero)
        {
            tracking.accelerateTimer = 0.1f;
        }
    }

    private bool GroundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up * (0.5f), Vector3.down, out hit, 0.75f, components.groundLayerMask))
        {
            if (hit.collider)
            {
                if (!tracking.canJump && !tracking.waitingToJump)
                {
                    tracking.waitingToJump = true;
                    StartCoroutine(DelayCanJump(0.5f));
                }

                if (!tracking.canDrop && !tracking.waitingToDrop)
                {
                    tracking.waitingToDrop = true;
                    StartCoroutine(DelayCanDrop(0.5f));
                }
                return true;
            }
        }
        tracking.airTime += Time.deltaTime;
        components.anim.SetFloat("AirTime", tracking.airTime);
        return false;
    }

    private void JumpFromLedge()
    {
        if (tracking.isGrounded && tracking.canJump)
        {
            bool foundGround = CheckAheadForGround();

            if (!foundGround)
            {
                OnJump();
            }
        }
    }

    private void OnJump()
    {
        components.rb.velocity = new Vector3(components.rb.velocity.x, 0f, components.rb.velocity.z) + transform.up * properties.jumpForce;
        components.anim.SetTrigger("Jump");
        tracking.isGrounded = false;
        tracking.canJump = false;
    }

    private void DropFromLedge()
    {
        if (tracking.isGrounded && tracking.canDrop)
        {
            bool foundGround = CheckAheadForGround();

            if (!foundGround)
            {
                OnDrop();
            }
        }
    }

    private void OnDrop()
    {
        components.rb.velocity = new Vector3(components.rb.velocity.x, 0f, components.rb.velocity.z) + transform.forward * 0.5f;
        components.anim.SetTrigger("Drop");
        tracking.isGrounded = false;
        tracking.canDrop = false;
    }

    private bool CheckAheadForGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up * (0.5f) + (transform.forward * 0.2f), Vector3.down, out hit, 0.75f, components.groundLayerMask))
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

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            tracking.isSprinting = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            tracking.isSprinting = false;
        }
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            tracking.actionButtonPressed = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            tracking.actionButtonPressed = false;
        }
    }

    public void OnToggleMainMenu(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (userInterfaces.mainMenu.activeSelf)
            {
                userInterfaces.mainMenu.SetActive(false);
                TogglePauseState(false);
                TriggerOverworldControls();
            }
            else if(currentState != characterStates.dialogue)
            {
                userInterfaces.mainMenu.SetActive(true);
                TogglePauseState(true);
                TriggerMainMenuControls();
            }
        }
    }

    public void TogglePauseState(bool toggle)
    {
        if(toggle)
        {
            currentMove = Vector2.zero;
            components.rb.velocity = Vector3.zero;
            components.anim.SetFloat("MoveY", 0);
            components.anim.SetFloat("TurnX", 0);
            currentState = characterStates.paused;
            components.freeLookCam.enabled = false;
        }
        else
        {
            currentState = characterStates.idle;
            components.freeLookCam.enabled = true;
        }
    }

    public void ToggleDialogueState(bool toggle)
    {
        if (toggle)
        {
            currentMove = Vector2.zero;
            components.rb.velocity *= 0.35f;
            components.anim.SetFloat("MoveY", 0);
            components.anim.SetFloat("TurnX", 0);
            currentState = characterStates.dialogue;
        }
        else
        {
            currentState = characterStates.idle;
        }
    }

    public void TriggerOverworldControls()
    {
        inputs.playerInputs.SwitchCurrentActionMap("Player");
    }

    public void TriggerMainMenuControls()
    {
        inputs.playerInputs.SwitchCurrentActionMap("Inventory");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Item>())
        {
            if (tracking.actionButtonPressed)
            {
                userInterfaces.itemDialogue.GetComponent<PickupScreen>().SetCurrentItem(other.GetComponent<Item>());
                userInterfaces.itemDialogue.SetActive(true);
            }
        }

        else if(other.GetComponent<PartyMember>())
        {
            PartyMember pMember = other.GetComponent<PartyMember>();
            if(tracking.actionButtonPressed)
            {
                if(!PartyManager.Instance.CheckIfInParty(pMember))
                {
                    userInterfaces.partyDialogue.GetComponent<PartyDialogue>().SetCharacter(pMember, false);
                    userInterfaces.partyDialogue.SetActive(true);
                }
            }
        }
    }

    private void Vaulting()
    {
        if (currentState == characterStates.vaulting)
        {
            if (parkour.canVaultOver)
            {
                transform.position = Vector3.Lerp(transform.position, parkour.targetPos, parkour.timer * parkour.vaultSpeed);
                if (parkour.timer >= 1f)
                {
                    currentState = characterStates.idle;
                    parkour.timer = 0;
                    components.rb.useGravity = true;
                    GetComponent<Collider>().enabled = true;
                    parkour.canVaultOver = false;
                }
                else
                {
                    parkour.timer += Time.deltaTime;
                }
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, parkour.vaultPeak, parkour.timer * 1f/parkour.floatDelay);
                if (parkour.timer >= parkour.floatDelay)
                {
                    parkour.timer = 0;
                }
                else
                {
                    parkour.timer += Time.deltaTime;
                }
                components.rb.velocity *= 0.35f;
            }
        }
        else if (VaultCheck())
        {
            parkour.targetPos = VaultTarget();
        }
    }

    private bool VaultCheck()
    {
        RaycastHit[] hits;
        Vector3 rayTemp = transform.position + (transform.forward * 0.75f) + (transform.up * parkour.maxVaultHeight);
        hits = Physics.RaycastAll(rayTemp, -transform.up, 2f, parkour.vaultLayer);
        if (hits.Length == 1)
        {
            float distance = Vector3.Distance(transform.position, hits[0].point);
            if (distance <= 1.5f && hits[0].collider.GetComponent<ActionPoint>())
            {
                if (hits[0].collider.GetComponent<ActionPoint>().action == ActionPoint.actionTypes.vault)
                {
                    Debug.DrawLine(rayTemp, hits[0].point, Color.green);

                    float height = hits[0].point.y - transform.position.y;
                    if (height >= 0.6f)
                    {
                        parkour.vaultPeak = transform.position + new Vector3(0, hits[0].point.y, 0);
                    }
                    else if (height < 0.6f)
                    {
                        parkour.vaultPeak = transform.position + new Vector3(0, hits[0].point.y, 0) * 0.5f;
                    }

                    return true;
                }
            }
        }
        Debug.DrawRay(rayTemp, -transform.up, Color.red, 1f);
        return false;
    }

    private Vector3 VaultTarget()
    {
        RaycastHit[] hits;
        Vector3 rayTemp = transform.position + (transform.forward * 0.75f) + (transform.up * parkour.maxVaultHeight);
        hits = Physics.RaycastAll(rayTemp, -transform.up, 2f, parkour.vaultLayer);
        if (hits.Length == 1)
        {
            float distance = 0.2f;
            Vector3 lastHit = new Vector3();
            bool hasTarget = false;
            bool isShortVault = false;
            RaycastHit[] targetHits;
            while (distance < 3f)
            {
                Vector3 targetRay = transform.position + (transform.up * 2f) + (transform.forward * distance);
                targetHits = Physics.RaycastAll(targetRay, -transform.up, 2f, parkour.vaultLayer);
                if (targetHits.Length == 1)
                {
                    lastHit = targetHits[0].point;
                    if (Vector3.Distance(transform.position, lastHit) >= 1.5f)
                    {
                        Debug.DrawLine(rayTemp, targetHits[0].point, Color.yellow, 3f);
                        hasTarget = true;
                        isShortVault = false;
                    }
                    else if (Vector3.Distance(transform.position, lastHit) >= 0.2f && Vector3.Distance(transform.position, lastHit) < 1.5f)
                    {
                        hasTarget = true;
                        isShortVault = true;
                    }
                    else
                    {
                        Debug.DrawLine(rayTemp, targetHits[0].point, Color.red, 3f);
                    }
                }
                else
                {
                    if(hasTarget)
                    {
                        Vector3 flatLastHit = new Vector3(lastHit.x, 0, lastHit.z);
                        Vector3 target = flatLastHit + transform.forward;
                        Debug.DrawLine(transform.position, target, Color.white, 3f);

                        if (isShortVault)
                        {
                            parkour.floatDelay = 0.1f;
                            StartCoroutine(VaultDelay(parkour.floatDelay, isShortVault));
                        }
                        else
                        {
                            parkour.floatDelay = 0.3f;
                            StartCoroutine(VaultDelay(parkour.floatDelay, isShortVault));
                        }
                        return target;
                    }
                }
                distance += 0.1f;
            }
        }
        return transform.position;
    }

    private IEnumerator VaultDelay(float delay, bool shortVault)
    {
        currentState = characterStates.vaulting;
        if (shortVault)
        {
            components.anim.SetTrigger("ShortVault");
        }
        else
        {
            components.anim.SetTrigger("Vault");
        }
        components.rb.useGravity = false;
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(delay);
        parkour.canVaultOver = true;
    }

    private IEnumerator DelayCanJump(float delay)
    {
        tracking.airTime = 0;
        yield return new WaitForSeconds(delay);
        tracking.canJump = true;
        tracking.waitingToJump = false;
    }

    private IEnumerator DelayCanDrop(float delay)
    {
        tracking.airTime = 0;
        yield return new WaitForSeconds(delay);
        tracking.canDrop = true;
        tracking.waitingToDrop = false;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (components.anim)
        {
            if (currentMove == Vector2.zero)
            {
                RaycastHit hit;

                //Left foot
                components.anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, components.anim.GetFloat("IKLeftFootWeight"));
                components.anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, components.anim.GetFloat("IKLeftFootWeight"));
                Ray ray = new Ray(components.anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
                if (Physics.Raycast(ray, out hit, properties.footDistanceToGround + 1f, components.groundLayerMask))
                {
                    if (hit.transform.tag == "Walkable")
                    {
                        Vector3 footPos = hit.point;
                        footPos.y += properties.footDistanceToGround;
                        components.anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPos);
                        components.anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation);
                    }
                }

                //Right foot
                components.anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, components.anim.GetFloat("IKRightFootWeight"));
                components.anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, components.anim.GetFloat("IKRightFootWeight"));
                ray = new Ray(components.anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
                if (Physics.Raycast(ray, out hit, properties.footDistanceToGround + 1f, components.groundLayerMask))
                {
                    if (hit.transform.tag == "Walkable")
                    {
                        Vector3 footPos = hit.point;
                        footPos.y += properties.footDistanceToGround;
                        components.anim.SetIKPosition(AvatarIKGoal.RightFoot, footPos);
                        components.anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation);
                    }
                }
            }
        }
    }
}
