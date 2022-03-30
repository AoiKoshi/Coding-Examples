using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutUI : MonoBehaviour
{
    private enum UIType
    {
        image,
        text,
        canvasGroup,
        blurMaterial
    }
    [SerializeField]
    private UIType elementType;
    [SerializeField]
    private float duration;
    [SerializeField]
    private bool onStart;

    private void OnEnable()
    {
        if (onStart)
        {
            Trigger();
        }
    }

    public void Trigger()
    {
        switch (elementType)
        {
            case UIType.image:
                StartCoroutine(UIAnimations.FadeOut(this.GetComponent<Image>(), duration));
                break;
            case UIType.text:
                StartCoroutine(UIAnimations.FadeOut(this.GetComponent<Text>(), duration));
                break;
            case UIType.canvasGroup:
                StartCoroutine(UIAnimations.FadeOut(this.GetComponent<CanvasGroup>(), duration));
                break;
            case UIType.blurMaterial:
                StartCoroutine(UIAnimations.FadeOut(this.GetComponent<Image>().material, duration));
                break;
        }
    }
}
