using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPoint : MonoBehaviour
{
    public enum actionTypes
    {
        vault,
        jump,
        jumpDown,
        swing,
        ladder,
        strafe,
        crawl
    }
    public actionTypes action;
}
