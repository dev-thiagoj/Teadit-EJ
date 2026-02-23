using Unity.Cinemachine;
using UnityEngine;

public class DynamicFreeLookDistance : MonoBehaviour
{
    public Transform targetRoot;
    public float padding = 1.2f;

    private CinemachineCamera cinemachineCam;
    private CinemachineOrbitalFollow orbital;
    private Camera unityCam;

    void Awake()
    {
        cinemachineCam = GetComponent<CinemachineCamera>();
        orbital = GetComponent<CinemachineOrbitalFollow>();
        unityCam = Camera.main;
    }

    void LateUpdate()
    {
        if (targetRoot == null || orbital == null) return;

        Bounds bounds = CalculateBounds();
        float radius = bounds.extents.magnitude;

        float fov = unityCam.fieldOfView * Mathf.Deg2Rad;
        float distance = (radius / Mathf.Sin(fov / 2f)) * padding;

        orbital.Radius = distance;
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
