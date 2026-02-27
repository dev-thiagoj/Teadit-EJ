using System;
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

    [Header("Dinamic Setup")]
    [SerializeField] bool dynamicFraming = true;
    [SerializeField] float framingSpeed = 5f;
    [SerializeField] float framingMargin = 1.5f;

    // dados iniciais para Reset View
    Vector3 initialFollowPosition;
    float initialRadius;

    void Awake()
    {
        input = new CADInputActions();
        orbital = GetComponent<CinemachineOrbitalFollow>();
    }

    void OnEnable()
    {
        input.Enable();
        jointManager.OnJointLoaded.AddListener(ReceiveJointContext);
        ExplosionUIController.OnResetViewed.RemoveListener(OnResetViewFromUI);
        ExplosionUIController.OnResetViewed.AddListener(OnResetViewFromUI);
        ExplosionUIController.OnExplosionChanged.AddListener(OnExplosionChanged);
    }

    void OnDisable()
    {
        input.Disable();
        jointManager.OnJointLoaded.RemoveListener(ReceiveJointContext);
        ExplosionUIController.OnResetViewed.RemoveListener(OnResetViewFromUI);
        ExplosionUIController.OnExplosionChanged.RemoveListener(OnExplosionChanged);
    }

    void Update()
    {
        HandleOrbit();
        HandlePan();
        HandleZoom();
    }

    //private void LateUpdate()
    //{
    //    HandleDynamicFraming();
    //}

    private void OnResetViewFromUI()
    {
        // 🔄 Reset pan
        followTarget.position = initialFollowPosition;

        // 🔄 Reset zoom
        orbital.Radius = initialRadius;
    }

    public void ReceiveJointContext(JointContext context)
    {
        if (context == null)
            return;

        modelCenter = context.TargetTransform; // use o target central calculado
        modelRoot = context.Instance.transform;

        // 🔹 Centraliza followTarget no centro do modelo
        followTarget.position = context.Center;

        float modelRadius = context.Radius;

        Camera cam = Camera.main;

        float verticalFOV = cam.fieldOfView * Mathf.Deg2Rad;
        float aspect = cam.aspect;

        // FOV horizontal real
        float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(verticalFOV / 2f) * aspect);

        // Calcula distância necessária verticalmente
        float distV = modelRadius / Mathf.Tan(verticalFOV / 2f);

        // Calcula distância necessária horizontalmente
        float distH = modelRadius / Mathf.Tan(horizontalFOV / 2f);

        // Usa a maior para garantir que nunca corte
        float distance = Mathf.Max(distV, distH);

        orbital.Radius = distance * 1.5f; // margem de 10%

        // 🔹 Ajusta limites dinamicamente
        maxPanDistance = modelRadius * 1.5f;
        minZoom = modelRadius * 0.5f;
        maxZoom = modelRadius * 15f;

        // 🔹 Salva estado inicial para Reset
        initialFollowPosition = followTarget.position;
        initialRadius = orbital.Radius;
    }

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
    private void OnExplosionChanged(float arg0)
    {
        if (modelRoot == null)
            return;

        Bounds bounds = CalculateDynamicBounds();
        float modelRadius = bounds.extents.magnitude;

        Camera cam = Camera.main;

        float verticalFOV = cam.fieldOfView * Mathf.Deg2Rad;
        float aspect = cam.aspect;
        float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(verticalFOV / 2f) * aspect);

        float distV = modelRadius / Mathf.Tan(verticalFOV / 2f);
        float distH = modelRadius / Mathf.Tan(horizontalFOV / 2f);

        float desiredDistance = Mathf.Max(distV, distH) * framingMargin;

        // anima suavemente até o novo valor
        orbital.Radius = Mathf.Lerp(orbital.Radius, desiredDistance, 0.6f);
    }

    void HandleDynamicFraming()
    {
        if (!dynamicFraming || modelRoot == null)
            return;

        Bounds bounds = CalculateDynamicBounds();
        float modelRadius = bounds.extents.magnitude;

        Camera cam = Camera.main;

        float verticalFOV = cam.fieldOfView * Mathf.Deg2Rad;
        float aspect = cam.aspect;
        float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(verticalFOV / 2f) * aspect);

        float distV = modelRadius / Mathf.Tan(verticalFOV / 2f);
        float distH = modelRadius / Mathf.Tan(horizontalFOV / 2f);

        float desiredDistance = Mathf.Max(distV, distH) * framingMargin;

        // 🔥 Movimento suave para MAIS OU PARA MENOS
        orbital.Radius = Mathf.Lerp(
            orbital.Radius,
            desiredDistance,
            Time.deltaTime * framingSpeed
        );
    }

    Bounds CalculateDynamicBounds()
    {
        Renderer[] renderers = modelRoot.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return new Bounds(modelRoot.position, Vector3.one);

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        return bounds;
    }

}
