using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Threadwork.Tools
{
    public class QuickTools : MonoBehaviour
    {
        public static void SetGlobalScale(Transform transform, Vector3 globalScale)
        {
            transform.localScale = Vector3.one;
            transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
        }
    }
}
