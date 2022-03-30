using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class UIAnimations : MonoBehaviour
{
    static public CanvasScaler canvasScaler;

    private void Awake()
    {
        canvasScaler = GetComponent<CanvasScaler>();
    }

    static public IEnumerator FadeIn(Text UIElement, float duration)
    {
        Color originalColor = UIElement.color;
        UIElement.color = new Color(0, 0, 0, 0);
        float timer = 0;

        while (timer < duration)
        {
            Color newColor = Color.Lerp(new Color(0, 0, 0, 0), originalColor, Mathf.SmoothStep(0f, 1f, timer / duration));
            UIElement.color = newColor;

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        UIElement.color = originalColor;
    }

    static public IEnumerator FadeIn(Image UIElement, float duration)
    {
        Color originalColor = UIElement.color;
        UIElement.color = new Color(0, 0, 0, 0);
        float timer = 0;

        while (timer < duration)
        {
            Color newColor = Color.Lerp(new Color(0, 0, 0, 0), originalColor, Mathf.SmoothStep(0f, 1f, timer / duration));
            UIElement.color = newColor;

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        UIElement.color = originalColor;
    }

    static public IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
    {
        canvasGroup.interactable = false;
        float timer = 0;
        while (timer < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, Mathf.SmoothStep(0f, 1f, timer / duration));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        canvasGroup.interactable = true;
    }

    static public IEnumerator FadeIn(Material mat, float duration)
    {
        float timer = 0;
        float opacity = 0;
        while (timer <= duration)
        {
            opacity = Mathf.Lerp(0f, 1f, Mathf.SmoothStep(0f, 1f, timer / duration));
            mat.SetFloat("opacity", opacity);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        mat.SetFloat("opacity", 1);
    }

    static public IEnumerator FadeOut(Text UIElement, float duration)
    {
        Color originalColor = UIElement.color;
        UIElement.color = new Color(0, 0, 0, 0);
        float timer = 0;

        while (timer < duration)
        {
            Color newColor = Color.Lerp(originalColor, new Color(0, 0, 0, 0), Mathf.SmoothStep(0f, 1f, timer / duration));
            UIElement.color = newColor;

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        UIElement.color = originalColor;
        UIElement.gameObject.SetActive(false);
    }

    static public IEnumerator FadeOut(Image UIElement, float duration)
    {
        Color originalColor = UIElement.color;
        UIElement.color = new Color(0, 0, 0, 0);
        float timer = 0;

        while (timer < duration)
        {
            Color newColor = Color.Lerp(originalColor, new Color(0, 0, 0, 0), Mathf.SmoothStep(0f, 1f, timer / duration));
            UIElement.color = newColor;

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        UIElement.color = originalColor;
        UIElement.gameObject.SetActive(false);
    }

    static public IEnumerator FadeOut(CanvasGroup canvasGroup, float duration)
    {
        canvasGroup.interactable = false;
        float timer = 0;
        while (timer < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, Mathf.SmoothStep(0f, 1f, timer / duration));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        canvasGroup.interactable = true;
    }

    static public IEnumerator FadeOut(Material mat, float duration)
    {
        float timer = 0;
        float opacity = 1;
        while (timer <= duration)
        {
            opacity = Mathf.Lerp(1, 0, Mathf.SmoothStep(0f, 1f, timer / duration));
            mat.SetFloat("opacity", opacity);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        mat.SetFloat("opacity", 0);
    }

    static public IEnumerator SlideInFromAbove(Transform UIElement, Vector2 ogPos, float duration)
    {
        float timer = 0;
        RectTransform rt = (RectTransform)UIElement;
        float y = 0f;
        Vector2 upTarget = new Vector2(ogPos.x, y + rt.rect.height);
        while (timer <= duration)
        {
            UIElement.transform.localPosition = Vector2.Lerp(upTarget, ogPos, Mathf.SmoothStep(0f, 1f, timer / duration));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    static public IEnumerator SlideInFromRight(Transform UIElement, Vector2 ogPos, float duration)
    {
        float timer = 0;
        RectTransform rt = (RectTransform)UIElement;
        float x = canvasScaler.referenceResolution.x;
        Vector2 rightTarget = new Vector2(x + rt.rect.width, ogPos.y);
        while (timer <= duration)
        {
            UIElement.transform.localPosition = Vector2.Lerp(rightTarget, ogPos, Mathf.SmoothStep(0f, 1f, timer / duration));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    static public IEnumerator SlideInFromBelow(Transform UIElement, Vector2 ogPos, float duration)
    {
        float timer = 0;
        RectTransform rt = (RectTransform)UIElement;
        float y = -canvasScaler.referenceResolution.y;
        Vector2 downTarget = new Vector2(ogPos.x, y - rt.rect.height);
        while (timer <= duration)
        {
            UIElement.transform.localPosition = Vector2.Lerp(downTarget, ogPos, Mathf.SmoothStep(0f, 1f, timer / duration));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    static public IEnumerator SlideInFromLeft(Transform UIElement, Vector2 ogPos, float duration)
    {
        float timer = 0;
        RectTransform rt = (RectTransform)UIElement;
        float x = 0f - canvasScaler.referenceResolution.x / 2f;
        Vector2 leftTarget = new Vector2(x - rt.rect.width, ogPos.y);
        while (timer <= duration)
        {
            UIElement.transform.localPosition = Vector2.Lerp(leftTarget, ogPos, Mathf.SmoothStep(0f, 1f, timer / duration));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    static public IEnumerator ScaleUp(Transform UIElement, Vector2 ogScale, float duration)
    {
        float timer = 0;
        while (timer <= duration)
        {
            UIElement.localScale = Vector2.Lerp(Vector2.zero, ogScale, Mathf.SmoothStep(0f, 1f, timer / duration));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    static public IEnumerator ScaleDown(Transform UIElement, Vector2 ogScale, float duration)
    {
        float timer = 0;
        while (timer <= duration)
        {
            UIElement.localScale = Vector2.Lerp(ogScale, Vector2.zero, Mathf.SmoothStep(0f, 1f, timer / duration));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
