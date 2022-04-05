using UnityEngine;
using Unity.Netcode;
using Cinemachine;

namespace Threadwork.Gameplay.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [Header("Networked Variables")]
        public Vector3 Position;
        public Vector2 MovementInput;

        [Header("Client-side Variables")]
        public Animator Anim;
        public Rigidbody RBody;
        public Camera MainCam;
        [Tooltip("Attach the free look Cinemachine component.")]
        public CinemachineFreeLook freelookCam;

        [Header("Modules")]
        public Renderer mRenderer;
        public CameraAssigner CameraAssingerModule;
        public ReadInput ReadInputModule;
        public Locomotion LocomotionModule;

        [Header("Camera Transforms")]
        [SerializeField] private Transform hips;
        [SerializeField] private Transform head;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            RBody = GetComponent<Rigidbody>();
            RBody.freezeRotation = true;

            CameraAssingerModule.enabled = IsLocalPlayer;
            CameraAssingerModule.listener.enabled = IsLocalPlayer;

            if (IsLocalPlayer)
            {
                CameraAssingerModule.RegisterPlayer(transform, hips, head);
                return;
            }

            MainCam.enabled = false;
            freelookCam.enabled = false;
        }
    }
}
