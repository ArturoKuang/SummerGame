using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target;
    public Vector3 offset;
    public float distance = 12.0f;
    public float minDistance = 1f;
    public float maxDistance = 15.0f;
    public float smoothSpeed = 1.0f;
    public float scrollSensitivity = 1f;
    public Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (!target)
        {
            Debug.Log("No target set for camera");
            return;
        }
        float num = Input.GetAxis("Mouse ScrollWheel");
        distance -= num * scrollSensitivity;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Vector3 pos = target.position + offset;
        pos -= transform.forward * distance;

        transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smoothSpeed);

    }
}
