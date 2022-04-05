using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInUI : MonoBehaviour
{
    private enum UIType
    {
        image,
        text
    }
    [SerializeField]
    private UIType elementType;
    [SerializeField]
    private float duration;

    private void OnEnable()
    {
        switch (elementType)
        {
            case UIType.image:
                StartCoroutine(UIAnimations.FadeIn(UIAnimations.UIType.image, this.GetComponent<Image>(), duration));
                break;
            case UIType.text:
                StartCoroutine(UIAnimations.FadeIn(UIAnimations.UIType.text, this.GetComponent<Text>(), duration));
                break;
        }
    }
}
