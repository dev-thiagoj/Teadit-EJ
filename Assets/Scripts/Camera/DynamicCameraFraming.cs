using UnityEngine;

public class DynamicCameraFraming : MonoBehaviour
{
    public Transform targetRoot;
    public float padding = 1.2f;
    public float smooth = 10f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (targetRoot == null) return;

        Bounds bounds = CalculateBounds();

        float radius = bounds.extents.magnitude;
        float fov = cam.fieldOfView * Mathf.Deg2Rad;

        float distance = radius / Mathf.Sin(fov / 2f);
        distance *= padding;

        Vector3 direction = transform.forward;
        Vector3 targetPosition = bounds.center - direction * distance;

        if (Application.isPlaying)
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smooth);
        else
            transform.position = targetPosition;
    }

    Bounds CalculateBounds()
    {
        Renderer[] renderers = targetRoot.GetComponentsInChildren<Renderer>();

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        return bounds;
    }
}
