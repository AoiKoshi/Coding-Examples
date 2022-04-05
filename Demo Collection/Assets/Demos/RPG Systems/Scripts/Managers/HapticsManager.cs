using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HapticsManager : MonoBehaviour
{
    public static HapticsManager Instance { get; private set; }

    [SerializeField]
    private PlayerInput playerInput;

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

    private bool ControllerCheck()
    {
        if (playerInput.currentControlScheme.ToLower() == "gamepad")
        {
            return true;
        }
        return false;
    }

    public IEnumerator Popup()
    {
        if (ControllerCheck())
        {
            Gamepad.current.SetMotorSpeeds(0.25f, 0f);
            yield return new WaitForSeconds(0.1f);
            Gamepad.current.SetMotorSpeeds(0f, 0.75f);
            yield return new WaitForSeconds(0.2f);
            InputSystem.ResetHaptics();
        }
    }
}
