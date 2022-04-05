using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineFreeLook))]
public class FreeLookAddOn : MonoBehaviour
{
    [Range(0f, 10f)] public float lookSpeed = 1f;
    public bool invertY = false;
    private CinemachineFreeLook freeLookComponent;

    public void Start()
    {
        freeLookComponent = GetComponent<CinemachineFreeLook>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 lookMovement = context.ReadValue<Vector2>().normalized;
        lookMovement.y = invertY ? -lookMovement.y : lookMovement.y;

        lookMovement.x = lookMovement.x * 180f;

        freeLookComponent.m_XAxis.Value += lookMovement.x * lookSpeed * Time.deltaTime;
        freeLookComponent.m_YAxis.Value += lookMovement.y * lookSpeed * Time.deltaTime;
    }
}
