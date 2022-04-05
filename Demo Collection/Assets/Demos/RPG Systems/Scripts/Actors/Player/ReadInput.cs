using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReadInput : MonoBehaviour
{
    public PlayerInput PlayerInputs;
    public InputActionAsset ControlsAsset;

    public Vector2 InputDir;
    public Vector3 CameraDir;
    public bool ActionButtonPressed;
    public bool sprintHeld;
    public bool jumpHeld;

    void FixedUpdate()
    {
        CameraDir = Camera.main.transform.eulerAngles;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        InputDir = context.ReadValue<Vector2>();
    }

    public void OnAction(InputAction.CallbackContext context)
    {
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
        if (context.phase == InputActionPhase.Started)
        {
            sprintHeld = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            sprintHeld = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            jumpHeld = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            jumpHeld = false;
        }
    }

    public void TriggerOverworldControls()
    {
        PlayerInputs.SwitchCurrentActionMap("Player");
    }

    public void TriggerMainMenuControls()
    {
        PlayerInputs.SwitchCurrentActionMap("MainMenu");
    }

    public void TriggerPauseMenuControls()
    {
        PlayerInputs.SwitchCurrentActionMap("PauseMenu");
    }
}
