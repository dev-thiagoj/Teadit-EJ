using UnityEngine;

public class JointContext
{
    public GameObject Instance { get; private set; }
    public Bounds Bounds { get; private set; }
    public Vector3 Center => Bounds.center;
    public float Radius => Bounds.extents.magnitude;
    public Transform TargetTransform { get; private set; }
    public Transform OrbitPivot;
    public Transform PanPivot;

    public JointContext(GameObject instance)
    {
        Instance = instance;
        Bounds = CalculateBounds(instance);

        GameObject target = new GameObject("CameraTarget");
        target.transform.position = Bounds.center;
        target.transform.SetParent(instance.transform);

        TargetTransform = target.transform;
    }

    private Bounds CalculateBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.one);

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        return bounds;
    }
}
