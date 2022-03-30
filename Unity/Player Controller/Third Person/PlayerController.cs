using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerController : Controller
{
    public static PlayerController Instance { get; private set; }

    [Header("Camera")]
    [SerializeField]
    private Camera mainCam;
    [SerializeField] [Tooltip("Attach the free look Cinemachine component.")]
    private CinemachineFreeLook freeLookCam;

    [Header("Inputs")]
    [SerializeField]
    private PlayerInput playerInputs;
    [SerializeField]
    private InputActionAsset controls;
    public bool actionButtonPressed;

    [Serializable]
    public class UserInterfaces
    {
        [Header("Menus")]
        public GameObject mainMenu;
        public GameObject itemDialogue;
        public GameObject partyDialogue;
        public GameObject pauseMenu;
    }
    [SerializeField]
    private UserInterfaces UI = new UserInterfaces();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        locomotion = GetComponent<Locomotion>();
        wallet = GetComponent<Wallet>();
        mainCam = Camera.main;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        currentMove = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (locomotion != null)
        {
            if (locomotion.enableSprinting)
            {
                if (context.phase == InputActionPhase.Started)
                {
                    locomotion.isSprinting = true;
                }
                else if (context.phase == InputActionPhase.Canceled)
                {
                    locomotion.isSprinting = false;
                }
            }
        }
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            actionButtonPressed = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            actionButtonPressed = false;
        }
    }

    public void OnToggleMainMenu(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (UI.mainMenu.activeSelf)
            {
                UI.mainMenu.SetActive(false);
                TogglePauseState(false);
                TriggerOverworldControls();
            }
            else if (currentState != characterStates.dialogue)
            {
                UI.mainMenu.SetActive(true);
                TogglePauseState(true);
                TriggerMainMenuControls();
            }
        }
    }

    public void TogglePauseState(bool toggle)
    {
        if (toggle)
        {
            currentMove = Vector2.zero;
            rb.velocity = Vector3.zero;
            anim.SetFloat("MoveY", 0);
            anim.SetFloat("TurnX", 0);
            currentState = characterStates.paused;
            freeLookCam.enabled = false;
        }
        else
        {
            currentState = characterStates.active;
            freeLookCam.enabled = true;
        }
    }

    public void ToggleDialogueState(bool toggle)
    {
        if (toggle)
        {
            currentMove = Vector2.zero;
            rb.velocity *= 0.35f;
            anim.SetFloat("MoveY", 0);
            anim.SetFloat("TurnX", 0);
            currentState = characterStates.dialogue;
        }
        else
        {
            currentState = characterStates.active;
        }
    }

    public void TriggerOverworldControls()
    {
        playerInputs.SwitchCurrentActionMap("Player");
    }

    public void TriggerMainMenuControls()
    {
        playerInputs.SwitchCurrentActionMap("Inventory");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Item>())
        {
            if (actionButtonPressed)
            {
                UI.itemDialogue.GetComponent<PickupScreen>().SetCurrentItem(other.GetComponent<Item>());
                UI.itemDialogue.SetActive(true);
            }
        }

        else if (other.GetComponent<PartyMember>())
        {
            PartyMember pMember = other.GetComponent<PartyMember>();
            if (actionButtonPressed)
            {
                if (!PartyManager.Instance.CheckIfInParty(pMember))
                {
                    UI.partyDialogue.GetComponent<PartyDialogue>().SetCharacter(pMember, false);
                    UI.partyDialogue.SetActive(true);
                }
            }
        }
    }
}
