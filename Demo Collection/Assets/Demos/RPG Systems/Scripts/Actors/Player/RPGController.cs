using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class RPGController : Controller
{
    public static new RPGController Instance;

    [Header("Inputs")]
    [SerializeField] private PlayerInput playerInputs;
    [SerializeField] private InputActionAsset controls;

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


    private void Awake() => Instance = this;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        locomotion = GetComponent<Locomotion>();
        mainCam = Camera.main;
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
                TriggerMenuControls();
            }
        }
    }

    public void TogglePauseState(bool toggle)
    {
        if (toggle)
        {
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

    public void TriggerMenuControls()
    {
        playerInputs.SwitchCurrentActionMap("Menu");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Item>())
        {
            if (readInput.ActionButtonPressed)
            {
                UI.itemDialogue.GetComponent<PickupScreen>().SetCurrentItem(other.GetComponent<Item>());
                UI.itemDialogue.SetActive(true);
            }
        }

        else if (other.GetComponent<PartyMember>())
        {
            PartyMember pMember = other.GetComponent<PartyMember>();
            if (readInput.ActionButtonPressed)
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
