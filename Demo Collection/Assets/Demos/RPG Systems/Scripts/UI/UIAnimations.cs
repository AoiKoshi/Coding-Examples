using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimations : MonoBehaviour
{
    public enum UIType
    {
        image,
        text
    }

    static public IEnumerator FadeIn(UIType UIElementType, Text UIElement, float duration)
    {
        Color originalColor = UIElement.color;
        UIElement.color = new Color(0, 0, 0, 0);
        float timer = 0;

        while (timer < duration)
        {
            Color newColor = Color.Lerp(new Color(0, 0, 0, 0), originalColor, timer/duration);
            UIElement.color = newColor;

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        UIElement.color = originalColor;
    }

    static public IEnumerator FadeIn(UIType UIElementType, Image UIElement, float duration)
    {
        Color originalColor = UIElement.color;
        UIElement.color = new Color(0, 0, 0, 0);
        float timer = 0;

        while (timer < duration)
        {
            Color newColor = Color.Lerp(new Color(0, 0, 0, 0), originalColor, timer/duration);
            UIElement.color = newColor;

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        UIElement.color = originalColor;
    }

    static public IEnumerator FadeOut(UIType UIElementType, Text UIElement, float duration)
    {
        Color originalColor = UIElement.color;
        UIElement.color = new Color(0, 0, 0, 0);
        float timer = 0;

        while (timer < duration)
        {
            Color newColor = Color.Lerp(originalColor, new Color(0, 0, 0, 0), timer / duration);
            UIElement.color = newColor;

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        UIElement.color = originalColor;
        UIElement.gameObject.SetActive(false);
    }

    static public IEnumerator FadeOut(UIType UIElementType, Image UIElement, float duration)
    {
        Color originalColor = UIElement.color;
        UIElement.color = new Color(0, 0, 0, 0);
        float timer = 0;

        while (timer < duration)
        {
            Color newColor = Color.Lerp(originalColor, new Color(0, 0, 0, 0), timer / duration);
            UIElement.color = newColor;

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        UIElement.color = originalColor;
        UIElement.gameObject.SetActive(false);
    }
}
