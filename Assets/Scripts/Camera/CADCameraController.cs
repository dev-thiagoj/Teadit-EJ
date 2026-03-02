using Unity.Cinemachine;
using UnityEngine;

public class CADCameraController : MonoBehaviour
{
    CADInputActions input;

    [Header("References")]
    [SerializeField] private JointManager jointManager;
    [SerializeField] private CinemachineTargetGroup targetGroup;
    CinemachineCamera cinemachineCamera;
    CinemachineGroupFraming groupFraming;

    [Header("Controls")]
    [SerializeField] private float orbitSpeed = 0.2f;
    [SerializeField] private float panSpeed = 0.01f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] float minZoom = 1f;
    [SerializeField] float maxZoom = 10f;

    private Transform modelRoot;

    private Quaternion initialModelRotation;
    private Vector3 initialGroupPosition;
    private float initialCameraDistance;

    void Awake()
    {
        input = new CADInputActions();

        cinemachineCamera = GetComponent<CinemachineCamera>();
        groupFraming = GetComponent<CinemachineGroupFraming>();
    }

    void OnEnable()
    {
        input.Enable();
        jointManager.OnJointLoaded.AddListener(ReceiveJointContext);
        ExplosionUIController.OnResetViewed.AddListener(OnResetViewFromUI);
    }

    void OnDisable()
    {
        input.Disable();
        jointManager.OnJointLoaded.RemoveListener(ReceiveJointContext);
        ExplosionUIController.OnResetViewed.RemoveListener(OnResetViewFromUI);
    }

    void Update()
    {
        HandleOrbit();
        HandlePan();
        HandleZoom();
    }

    // =========================================================
    // CONTEXT RECEIVED
    // =========================================================

    public void ReceiveJointContext(JointContext context)
    {
        if (context == null)
            return;

        modelRoot = context.Instance.transform;

        initialModelRotation = modelRoot.localRotation;
        initialGroupPosition = targetGroup.transform.position;
        initialCameraDistance = groupFraming.FramingSize;
    }

    // =========================================================
    // ORBIT (rotaciona o modelo, não a câmera)
    // =========================================================

    void HandleOrbit()
    {
        if (modelRoot == null)
            return;

        if (!input.Camera.OrbitButton.IsPressed())
            return;

        Vector2 delta = input.Camera.Orbit.ReadValue<Vector2>();

        float rotX = delta.y * orbitSpeed;
        float rotY = -delta.x * orbitSpeed;

        modelRoot.Rotate(Vector3.up, rotY, Space.World);
        modelRoot.Rotate(Vector3.right, rotX, Space.World);
    }

    // =========================================================
    // PAN (move o TargetGroup)
    // =========================================================

    void HandlePan()
    {
        if (!input.Camera.PanButton.IsPressed())
            return;

        Vector2 delta = input.Camera.Pan.ReadValue<Vector2>();
        if (delta.sqrMagnitude < 0.001f)
            return;

        // CenterOffset no Cinemachine 3 é um Vector2 (X, Y)
        // Valores menores (ex: 0.005) costumam funcionar melhor aqui
        groupFraming.CenterOffset.x += delta.x * panSpeed;
        groupFraming.CenterOffset.y += delta.y * panSpeed;
    }

    // =========================================================
    // ZOOM (controla CameraDistance do Framing Transposer)
    // =========================================================

    void HandleZoom()
    {
        float scroll = input.Camera.Zoom.ReadValue<float>();

        if (Mathf.Abs(scroll) < 0.01f)
            return;

        // No FramingSize: Menor = Mais perto (Zoom In), Maior = Mais longe (Zoom Out)
        // Inverti o sinal do scroll para scroll up = aproximar
        groupFraming.FramingSize += scroll * zoomSpeed * Time.deltaTime;

        groupFraming.FramingSize = Mathf.Clamp(
            groupFraming.FramingSize,
            minZoom,
            maxZoom
        );
    }

    // =========================================================
    // RESET VIEW
    // =========================================================

    private void OnResetViewFromUI()
    {
        if (modelRoot == null)
            return;

        modelRoot.localRotation = initialModelRotation;

        // Reseta o deslocamento do Pan
        groupFraming.CenterOffset = Vector2.zero;

        // Reseta o Zoom para o valor salvo no ReceiveJointContext
        groupFraming.FramingSize = initialCameraDistance;
    }

}
