using UnityEngine;
using System.Collections;

public class ObjectFollow : MonoBehaviour {

    private Vector3 velocity;
    [SerializeField]
    private float smoothTimeY;
    [SerializeField]
    private float smoothTimeX;
    [SerializeField]
    private float smoothTimeZ;

    [SerializeField]
    private GameObject targetObject;

    void FixedUpdate()
    {
        float posX = Mathf.SmoothDamp(transform.position.x, targetObject.transform.position.x, ref velocity.x, smoothTimeX);
        float posY = Mathf.SmoothDamp(transform.position.y, targetObject.transform.position.y, ref velocity.y, smoothTimeY);
        float posZ = Mathf.SmoothDamp(transform.position.z, targetObject.transform.position.z, ref velocity.z, smoothTimeZ);
        transform.position = new Vector3(posX, posY, posZ);
    }
}
