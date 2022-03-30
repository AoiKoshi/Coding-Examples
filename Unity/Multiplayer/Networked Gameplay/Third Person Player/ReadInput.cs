using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;


public class ReadInput : NetworkBehaviour
{
    public PlayerInput PlayerInputs;
    public InputActionAsset ControlsAsset;

    public Vector2 InputDir;
    public Vector3 CameraDir;
    public bool ActionButtonPressed;
    public bool sprintHeld;

    void FixedUpdate()
    {
        if (!IsLocalPlayer) return;

        CameraDir = Camera.main.transform.eulerAngles;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsLocalPlayer) return;
        InputDir = context.ReadValue<Vector2>();
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        if (!IsLocalPlayer) return;

        if (context.phase == InputActionPhase.Performed)
        {
            ActionButtonPressed = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            ActionButtonPressed = false;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (!IsLocalPlayer) return;

        if (context.phase == InputActionPhase.Started)
        {
            sprintHeld = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            sprintHeld = false;
        }
    }

    public void TriggerOverworldControls()
    {
        if (!IsLocalPlayer) return;
        PlayerInputs.SwitchCurrentActionMap("Player");
    }

    public void TriggerMainMenuControls()
    {
        if (!IsLocalPlayer) return;
        PlayerInputs.SwitchCurrentActionMap("MainMenu");
    }

    public void TriggerPauseMenuControls()
    {
        if (!IsLocalPlayer) return;
        PlayerInputs.SwitchCurrentActionMap("PauseMenu");
    }
}
