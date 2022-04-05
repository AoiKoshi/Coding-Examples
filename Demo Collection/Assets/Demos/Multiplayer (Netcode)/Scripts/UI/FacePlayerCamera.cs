using UnityEngine;

public class FacePlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private float distanceFromCamera;

    private void Update()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(new Vector3(0, 180, 0));
    }
}
