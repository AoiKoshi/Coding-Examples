using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Cinemachine;
using System;

namespace Threadwork.Gameplay.Player
{
    public class CameraAssigner : MonoBehaviour
    {
        public Camera mainCam;
        public CinemachineFreeLook freelookCam;
        public AudioListener listener;

        public void ToggleCameras(bool toggle)
        {
            mainCam.enabled = toggle;
            freelookCam.enabled = toggle;
        }

        public void RegisterPlayer(Transform target, Transform midTarget, Transform topTarget)
        {
            freelookCam.LookAt = target;
            freelookCam.Follow = target;

            freelookCam.GetRig(0).LookAt = topTarget;
            freelookCam.GetRig(1).LookAt = midTarget;
            freelookCam.GetRig(2).LookAt = midTarget;
        }
    }
}
