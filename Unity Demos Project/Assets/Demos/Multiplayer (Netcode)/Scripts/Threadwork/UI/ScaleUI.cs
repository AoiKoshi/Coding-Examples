using UnityEngine;

namespace Threadwork.UI
{
    public class ScaleUI : MonoBehaviour
    {
        public enum scaleType
        {
            scaleUp,
            scaleDown
        }
        public scaleType type;

        [SerializeField] private float duration;
        [SerializeField] private bool onStart;

        private Vector2 ogScale;

        private void OnEnable()
        {
            ogScale = transform.localScale;
            if (onStart) Trigger();
        }

        public void Trigger()
        {
            switch (type)
            {
                case scaleType.scaleUp:
                    StartCoroutine(UIAnimations.ScaleUp(transform, ogScale, duration));
                    break;
                case scaleType.scaleDown:
                    StartCoroutine(UIAnimations.ScaleDown(transform, ogScale, duration));
                    break;
            }
        }
    }
}
