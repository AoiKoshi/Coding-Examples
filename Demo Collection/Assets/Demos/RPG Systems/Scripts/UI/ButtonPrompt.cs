using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonPrompt : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private GameObject controllerPrompts;
    [SerializeField]
    private GameObject kbmPrompts;

    public void OnSwitchControls()
    {
        switch (playerInput.currentControlScheme.ToLower())
        {
            case "gamepad":
                SwitchToController();
                break;
            case "keyboard&mouse":
                SwitchToKBM();
                break;
        }
    }

    public void SwitchToController()
    {
        controllerPrompts.SetActive(true);
        kbmPrompts.SetActive(false);
    }

    public void SwitchToKBM()
    {
        controllerPrompts.SetActive(false);
        kbmPrompts.SetActive(true);
    }
}
