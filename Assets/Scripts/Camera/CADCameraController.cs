using Unity.Cinemachine;
using UnityEngine;

public class CADCameraController : MonoBehaviour
{
    CADInputActions input;
    CinemachineOrbitalFollow orbital;
    [SerializeField] JointManager jointManager;

    [SerializeField] float orbitSpeed = 0.2f;
    [SerializeField] float panSpeed = 0.01f;
    [SerializeField] float zoomSpeed = 5f;

    [Header("Pan Setup")]
    [SerializeField] Transform followTarget;
    [SerializeField] Transform modelCenter;
    [SerializeField] float maxPanDistance = 5f;

    [Header("Zoom Setup")]
    [SerializeField] float minZoom = 1f;
    [SerializeField] float maxZoom = 10f;

    void Awake()
    {
        input = new CADInputActions();
        orbital = GetComponent<CinemachineOrbitalFollow>();
    }

    void OnEnable()
    {
        input.Enable();
        jointManager.OnJointLoaded.AddListener(ReceiveJointContext);
    }

    void OnDisable()
    {
        input.Disable();
        jointManager.OnJointLoaded.RemoveListener(ReceiveJointContext);
    }

    void Update()
    {
        HandleOrbit();
        HandlePan();
        HandleZoom();
    }

    public void ReceiveJointContext(JointContext context)
    {
        modelCenter = context.OrbitPivot;
        modelRoot = context.Instance.transform;
    }

    //void HandleOrbit()
    //{
    //    if (!input.Camera.OrbitButton.IsPressed()) return;

    //    Vector2 delta = input.Camera.Orbit.ReadValue<Vector2>();

    //    orbital.HorizontalAxis.Value += delta.x * orbitSpeed;
    //    orbital.VerticalAxis.Value -= delta.y * orbitSpeed;
    //}

    Transform modelRoot;
    void HandleOrbit()
    {
        if (!input.Camera.OrbitButton.IsPressed()) return;

        Vector2 delta = input.Camera.Orbit.ReadValue<Vector2>();

        float rotX = delta.y * orbitSpeed;
        float rotY = -delta.x * orbitSpeed;

        modelRoot.Rotate(Vector3.up, rotY, Space.World);
        modelRoot.Rotate(Vector3.right, rotX, Space.World);
    }

    void HandlePan()
    {
        if (!input.Camera.PanButton.IsPressed()) return;

        Vector2 delta = input.Camera.Pan.ReadValue<Vector2>();
        if (delta.sqrMagnitude < 0.001f) return;

        Vector3 right = Camera.main.transform.right;
        Vector3 up = Camera.main.transform.up;

        Vector3 move = (-right * delta.x - up * delta.y) * panSpeed;

        followTarget.position += move;

        // 🔒 Limite de distância
        Vector3 offset = followTarget.position - modelCenter.position;

        if (offset.magnitude > maxPanDistance)
        {
            followTarget.position =
                modelCenter.position + offset.normalized * maxPanDistance;
        }
    }

    void HandleZoom()
    {
        float scroll = input.Camera.Zoom.ReadValue<float>();
        if (Mathf.Abs(scroll) < 0.01f) return;

        orbital.Radius -= scroll * zoomSpeed * Time.deltaTime;

        orbital.Radius = Mathf.Clamp(orbital.Radius, minZoom, maxZoom);
    }
}
