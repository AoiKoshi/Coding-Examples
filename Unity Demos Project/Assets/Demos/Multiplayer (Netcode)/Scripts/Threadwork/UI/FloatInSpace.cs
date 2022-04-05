using UnityEngine;

namespace Threadwork.UI
{
    public class FloatInSpace : MonoBehaviour
    {
        [SerializeField] private float degreesPerSecond;
        [SerializeField] private float maxDegrees;
        [SerializeField] private float minDegrees;
        [SerializeField] private float amplitude;
        [SerializeField] private float frequency;

        private Vector3 originalPos;
        private Vector3 tempPos;
        private bool isRotatingRight = true;

        void Start()
        {
            originalPos = transform.position;
        }

        void Update()
        {
            Rotating(isRotatingRight);
        }

        private void Rotating(bool isRight)
        {
            float direction = isRight ? 1 : -1;
            transform.Rotate(new Vector3(0f, direction * Time.deltaTime * degreesPerSecond, 0f), Space.World);
            switch (isRotatingRight)
            {
                case true:
                    if (transform.rotation.eulerAngles.y > maxDegrees && transform.rotation.eulerAngles.y < maxDegrees + 1)
                    {
                        isRotatingRight = false;
                    }
                    break;

                case false:
                    if (transform.rotation.eulerAngles.y < minDegrees && transform.rotation.eulerAngles.y > minDegrees - 1)
                    {
                        isRotatingRight = true;
                    }
                    break;
            }

            tempPos = originalPos;
            tempPos.y += Mathf.Sin(Time.time * Mathf.PI * frequency) * amplitude;
            transform.position = tempPos;
        }
    }
}
