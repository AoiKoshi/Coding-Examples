using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideInUI : MonoBehaviour
{
    public enum directions
    {
        up,
        right,
        down,
        left
    }
    public directions direction;
    [SerializeField]
    private float duration;
    [SerializeField]
    private bool onStart;
    private Vector2 ogPos;

    private void OnEnable()
    {
        ogPos = transform.localPosition;
        if (onStart)
        {
            Trigger();
        }
    }

    public void Trigger()
    {
        switch (direction)
        {
            case directions.up:
                StartCoroutine(UIAnimations.SlideInFromAbove(transform, ogPos, duration));
                break;
            case directions.right:
                StartCoroutine(UIAnimations.SlideInFromRight(transform, ogPos, duration));
                break;
            case directions.down:
                StartCoroutine(UIAnimations.SlideInFromBelow(transform, ogPos, duration));
                break;
            case directions.left:
                StartCoroutine(UIAnimations.SlideInFromLeft(transform, ogPos, duration));
                break;
        }
    }
}
