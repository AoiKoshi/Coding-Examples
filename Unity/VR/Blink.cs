using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Blink : MonoBehaviour
{
    [SerializeField] private float blinkInterval;
    [SerializeField] private CanvasGroup blinkImage;

    private Volume volume;
    private Vignette vignette;
    public static Blink Instance;
    private float timer;

    private enum blinkStates
    {
        normal,
        open,
        close
    }
    private blinkStates blinkState;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            this.enabled = false;
        }
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out vignette);
    }

    void Update()
    {
        float min, max;
        float lerpValue, lerpValue2;
        blinkStates changeToState = blinkStates.normal;

        switch (blinkState)
        {
            case blinkStates.close:
                lerpValue = timer / blinkInterval;
                lerpValue2 = timer / blinkInterval * 0.5f;
                min = 0f;
                max = 1f;
                changeToState = blinkStates.open;
                break;

            case blinkStates.open:
                lerpValue = timer / blinkInterval * 0.5f;
                lerpValue2 = timer / blinkInterval;
                min = 1f;
                max = 0f;
                changeToState = blinkStates.normal;
                break;
        }

        if (changeToState != blinkStates.normal)
        {
            timer += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(min, max, lerpValue);
            blinkImage.alpha = Mathf.Lerp(min, max, lerpValue2);

            if (timer / blinkInterval) >= 2f)
            {
                blinkState = changeToState;
                timer = 0f;
            }
        }
    }

    [ContextMenu("Blink")]
    public void OnBlink()
    {
        timer = 0f;
        vignette.intensity.value = 0f;
        blinkImage.alpha = 0f;
        blinkState = blinkStates.close;
    }
}