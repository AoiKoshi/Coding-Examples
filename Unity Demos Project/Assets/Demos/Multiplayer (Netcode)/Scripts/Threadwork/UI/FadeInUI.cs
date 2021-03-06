using UnityEngine;
using UnityEngine.UI;

namespace Threadwork.UI
{
    public class FadeInUI : MonoBehaviour
    {
        private enum UIType
        {
            image,
            text,
            canvasGroup,
            blurMaterial
        }
        [SerializeField] private UIType elementType;

        [SerializeField] private float duration;
        [SerializeField] private bool onStart;

        private void OnEnable()
        {
            if (onStart) Trigger();
        }

        public void Trigger()
        {
            switch (elementType)
            {
                case UIType.image:
                    StartCoroutine(UIAnimations.FadeIn(this.GetComponent<Image>(), duration));
                    break;
                case UIType.text:
                    StartCoroutine(UIAnimations.FadeIn(this.GetComponent<Text>(), duration));
                    break;
                case UIType.canvasGroup:
                    StartCoroutine(UIAnimations.FadeIn(this.GetComponent<CanvasGroup>(), duration));
                    break;
                case UIType.blurMaterial:
                    StartCoroutine(UIAnimations.FadeIn(this.GetComponent<Image>().material, duration));
                    break;
            }
        }
    }
}
