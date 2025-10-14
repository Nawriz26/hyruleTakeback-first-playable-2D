using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 1.5f, -10);
    public float smooth = 8f;

    void LateUpdate()
    {
        if (!target) return;
        Vector3 desired = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smooth);
    }
}
