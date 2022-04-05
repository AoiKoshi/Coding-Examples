using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutUI : MonoBehaviour
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
                StartCoroutine(UIAnimations.FadeOut(UIAnimations.UIType.image, this.GetComponent<Image>(), duration));
                break;
            case UIType.text:
                StartCoroutine(UIAnimations.FadeOut(UIAnimations.UIType.text, this.GetComponent<Text>(), duration));
                break;
        }
    }
}
