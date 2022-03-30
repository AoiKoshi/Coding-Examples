using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabController : MonoBehaviour
{
    [Header("Grab visuals")]
    [SerializeField] private Hand hand;
    [SerializeField] private GameObject grabMarker;

    [Header("Grab targets")]
    [SerializeField] private Transform holdTarget;

    [Header("Grab attributes")]
    [SerializeField] private handTypes handType;
    [SerializeField]
    private enum handTypes
    { 
        right,
        left
    }
    [SerializeField] private LayerMask grabLayerMask;
    [SerializeField] private float grabRange = 0.5f;
    [SerializeField] private float pullSpeed = 1f;
    [SerializeField] private float pullAngle = 270f;
    [SerializeField] private float throwForce = 10f;

    //Grab physics
    private Rigidbody handRB;
    private Vector3 centreOfMass;
    private Vector3 PrevPos;
    private Vector3 NewPos;
    private Vector3 handVelocity;
    Quaternion rotationLast;
    float rotMagnitude;
    Vector3 rotAxis;
    Vector3 handAngularVelocity
    {
        get
        {
            return (rotAxis * rotMagnitude) / Time.deltaTime;
        }
    }
    private GameObject potentialGrab;
    private GameObject currentlyGrabbing;
    private Vector3[] VelocityFrames = new Vector3[5];
    private Vector3[] AngularVelocityFrames = new Vector3[5];
    private int currentVelocityFrameStep;

    void Start()
    {
        handRB = GetComponent<Rigidbody>();
        centreOfMass = handRB.centerOfMass;

        PrevPos = transform.position;
        NewPos = transform.position;
        rotationLast = transform.rotation;
    }

    void Update()
    {
        List<UnityEngine.XR.InputDevice> controllers = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDeviceCharacteristics desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.None;
        switch (handType)
        {
            case handTypes.left:
                desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Left | UnityEngine.XR.InputDeviceCharacteristics.Controller;
                break;
            case handTypes.right:
                desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller;
                break;
        }
        ProcessHandController(controllers, desiredCharacteristics);


        if (currentlyGrabbing != null)
        {
            OnGrabHold();
        }
        else
        {
            GloveCast();
        }
    }

    void FixedUpdate()
    {
        NewPos = transform.position;
        handVelocity = (NewPos - PrevPos) / Time.deltaTime;
        PrevPos = NewPos;

        Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(rotationLast);
        deltaRotation.ToAngleAxis(out rotMagnitude, out rotAxis);
        rotationLast = transform.rotation;

        TrackHandVelocity();
    }

    private void ProcessHandController(List<UnityEngine.XR.InputDevice> controllers, UnityEngine.XR.InputDeviceCharacteristics desiredCharacteristics)
    {
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, controllers);

        foreach (var device in controllers)
        {
            Vector3 position;
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out position))
            {
                transform.localPosition = position;
            }

            Quaternion orientation;
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out orientation))
            {
                transform.localRotation = orientation;
            }

            bool tpValue;
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out tpValue) && tpValue)
            {
                tpLine.enabled = true;
                tpPoint.SetActive(true);
            }
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out tpValue) && !tpValue)
            {
                tpLine.enabled = false;
                tpPoint.SetActive(false);
            }

            bool triggerValue;
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out triggerValue) && triggerValue)
            {
                if (potentialGrab != null && currentlyGrabbing == null)
                {
                    OnGrab();
                }
            }

            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out triggerValue) && !triggerValue)
            {
                if (currentlyGrabbing != null)
                {
                    OnGrabExit();
                }
            }
        }
    }

    private void GloveCast()
    {
        float range = hasGloves ? magnetRange : grabRange;
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit, range, grabLayerMask);
        Debug.DrawLine(transform.position, transform.position + (transform.forward * grabRange));
        if (hit.collider == null)
        {
            if(!isHolstered)
            {
                grabMarker.SetActive(false);
                potentialGrab = null;
            }
            return;
        }
        MoveMarker(grabMarker, hit.collider.transform);
        potentialGrab = hit.collider.gameObject;
    }

    private void MoveMarker(GameObject marker, Transform grabbableObject)
    {
        marker.SetActive(true);
        marker.transform.SetParent(grabbableObject);
        marker.transform.localPosition = Vector3.zero;
        marker.transform.LookAt(Camera.main.transform);
    }

    private void OnGrab()
    {
        currentlyGrabbing = potentialGrab;

        currentlyGrabbing.GetComponent<Rigidbody>().useGravity = false;
        Grabbable grabbable = currentlyGrabbing.GetComponent<Grabbable>();
        if (grabbable != null)
        {
            grabbable.EnableHint(true);
        }
        hand.ToggleHand(false);
        GrabHaptic();
        grabMarker.SetActive(false);
    }

    private void OnGrabHold()
    {
        Vector3 targetPos = transform.position + handOffset;
        currentlyGrabbing.transform.position = Vector3.MoveTowards(currentlyGrabbing.transform.position, holdTarget.position, pullSpeed * Time.deltaTime);
        currentlyGrabbing.transform.rotation = Quaternion.RotateTowards(currentlyGrabbing.transform.rotation, transform.rotation, pullAngle * Time.deltaTime);
    }

    private void TrackHandVelocity()
    {
        if (VelocityFrames != null)
        {
            currentVelocityFrameStep++;
            if (currentVelocityFrameStep >= VelocityFrames.Length)
            {
                currentVelocityFrameStep = 0;
            }
            VelocityFrames[currentVelocityFrameStep] = handVelocity;
            AngularVelocityFrames[currentVelocityFrameStep] = handAngularVelocity;
        }
    }

    private Vector3 GetVectorAverage(Vector3[] frames)
    {
        Vector3 average = new Vector3();
        int numOfVectors = 0;

        foreach (Vector3 frame in frames)
        {
            average += frame;
            numOfVectors++;
        }
        average /= numOfVectors;

        if (numOfVectors > 0)
        {
            return average;
        }
        return Vector3.zero;
    }

    private void AddVelocityHistory()
    {
        if (VelocityFrames != null)
        {
            Rigidbody itemRB = currentlyGrabbing.GetComponent<Rigidbody>();

            Vector3 velocityAverage = GetVectorAverage(VelocityFrames);
            if (velocityAverage != Vector3.zero)
            {
                itemRB.velocity = velocityAverage * throwForce;
                Debug.Log($"Velocity is: {velocityAverage * throwForce}");
            }
        }
    }

    private void ResetVelocityHistory()
    {
        currentVelocityFrameStep = 0;
        if (VelocityFrames != null && VelocityFrames.Length > 0)
        {
            VelocityFrames = new Vector3[VelocityFrames.Length];
            AngularVelocityFrames = new Vector3[AngularVelocityFrames.Length];
        }
    }

    private void OnThrow()
    {
        AddVelocityHistory();
        ResetVelocityHistory();
    }

    private void OnGrabExit()
    {
        currentlyGrabbing.GetComponent<Rigidbody>().useGravity = true;
        Grabbable grabbable = currentlyGrabbing.GetComponent<Grabbable>();
        if (grabbable != null)
        {
            grabbable.EnableHint(false);
        }

        OnThrow();

        hand.ToggleHand(true);
        potentialGrab = null;
        currentlyGrabbing = null;
    }

    private void GrabHaptic()
    {
        switch (handType)
        {
            case handTypes.left:
                PlayerManager.Instance.TriggerHaptic(0.7f, 0.05f, PlayerManager.handTypes.left);
                break;
            case handTypes.right:
                PlayerManager.Instance.TriggerHaptic(0.7f, 0.05f, PlayerManager.handTypes.right);
                break;
        }
    }
}
