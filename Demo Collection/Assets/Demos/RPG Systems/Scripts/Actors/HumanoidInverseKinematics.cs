using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidInverseKinematics : ControllerModule
{
    [Header("Inverse Kinematics")]
    [Tooltip("Adjust IK foot positioning.")] [Range(0, 1)]
    public float footDistanceToGround;
    [Tooltip("Adjust IK hand positioning.")] [Range(0, 1)]
    public float handDistanceFromObject;

    private void OnAnimatorIK(int layerIndex)
    {
        if (controller.anim)
        {
            if (controller.readInput.InputDir == Vector2.zero)
            {
                RaycastHit hit;

                //Left foot
                controller.anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, controller.anim.GetFloat("IKLeftFootWeight"));
                controller.anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, controller.anim.GetFloat("IKLeftFootWeight"));
                Ray ray = new Ray(controller.anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
                if (Physics.Raycast(ray, out hit, footDistanceToGround + 1f, controller.GetGroundLayer()))
                {
                    if (hit.transform.tag == "Walkable")
                    {
                        Vector3 footPos = hit.point;
                        footPos.y += footDistanceToGround;
                        controller.anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPos);
                        controller.anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation);
                    }
                }

                //Right foot
                controller.anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, controller.anim.GetFloat("IKRightFootWeight"));
                controller.anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, controller.anim.GetFloat("IKRightFootWeight"));
                ray = new Ray(controller.anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
                if (Physics.Raycast(ray, out hit, footDistanceToGround + 1f, controller.GetGroundLayer()))
                {
                    if (hit.transform.tag == "Walkable")
                    {
                        Vector3 footPos = hit.point;
                        footPos.y += footDistanceToGround;
                        controller.anim.SetIKPosition(AvatarIKGoal.RightFoot, footPos);
                        controller.anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation);
                    }
                }
            }
        }
    }
}
