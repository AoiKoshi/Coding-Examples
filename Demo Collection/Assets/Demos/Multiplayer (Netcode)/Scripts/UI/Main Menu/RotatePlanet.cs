using UnityEngine;

public class RotatePlanet : MonoBehaviour
{
    [SerializeField] private float speed = 0.2f;

    private void FixedUpdate()
    {
        gameObject.transform.Rotate(0f, speed, 0f);
    }
}
