using Unity.Cinemachine;
using UnityEngine;

public class NewCADCameraController : MonoBehaviour
{
    private CADInputActions input;

    [Header("References")]
    [SerializeField] private JointManager jointManager;
    [Tooltip("Um GameObject vazio no centro da tela. Usado apenas para o Pan.")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private CinemachineCamera cinemachineCamera;

    private CinemachineThirdPersonFollow thirdPersonFollow;
    private Transform modelRoot; // Referência da sua peça para girar

    [Header("Controls")]
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 20f;

    [Header("Orbit Settings")]
    [SerializeField] private float orbitSpeed = 0.2f;
    [Tooltip("Se verdadeiro, trava a rotação para acontecer apenas no eixo Y (giro lateral).")]
    [SerializeField] private bool lockRotationY = false;
    private enum AxisLock { None, Horizontal, Vertical }
    private AxisLock currentLock = AxisLock.None;
    private float currentRotX;
    private float currentRotY;

    [Header("Pan Settings (Relativo ao Início)")]
    [SerializeField] private float panSpeed = 0.05f;
    [Tooltip("Distância máxima para a esquerda/direita a partir do centro inicial.")]
    [SerializeField] private float maxPanX = 5f;
    [Tooltip("Distância máxima que a câmera pode SUBIR a partir do ponto inicial (ex: 2).")]
    [SerializeField] private float panLimitUp = 2f;
    [Tooltip("Distância máxima que a câmera pode DESCER a partir do ponto inicial (ex: 0.5).")]
    [SerializeField] private float panLimitDown = 1f;
    [Tooltip("Inverte a direção do movimento horizontal do Pan.")]
    [SerializeField] private bool invertPan = true;

    private float currentDistance = 2f;
    private Vector3 initialPivotPosition;
    private Quaternion initialModelRotation;
    private Vector3 initialShoulderOffset;
      
    void OnEnable()
    {
        input.Enable();
        jointManager.OnJointLoaded.AddListener(ReceiveJointContext);
        ExplosionUIController.OnResetViewed.AddListener(ResetView);
    }

    void OnDisable()
    {
        input.Disable();
        jointManager.OnJointLoaded.RemoveListener(ReceiveJointContext);
        ExplosionUIController.OnResetViewed.RemoveListener(ResetView);
    }

    void Awake()
    {
        input = new CADInputActions();

        if (cinemachineCamera != null)
        {
            thirdPersonFollow = cinemachineCamera.GetComponent<CinemachineThirdPersonFollow>();
            if (thirdPersonFollow != null)
            {
                currentDistance = thirdPersonFollow.CameraDistance;
                // Guarda o offset inicial (geralmente é Zero, mas é bom garantir)
                initialShoulderOffset = thirdPersonFollow.ShoulderOffset;
            }
        }

        if (cameraPivot != null)
            initialPivotPosition = cameraPivot.position;
    }

    private void Start()
    {
        if (modelRoot != null)
        {
            currentRotX = modelRoot.eulerAngles.x;
            currentRotY = modelRoot.eulerAngles.y;
        }
    }

    // Carrega a peça como no seu script original
    public void ReceiveJointContext(JointContext context)
    {
        if (context == null) return;

        modelRoot = context.Instance.transform;
        initialModelRotation = modelRoot.localRotation;
    }

    void LateUpdate()
    {
        HandleOrbit();
        HandlePan();
        HandleZoom();
    }

    private void HandleOrbit()
    {
        if (modelRoot == null) return;

        // Reseta a trava ao soltar o clique
        if (!input.Camera.OrbitButton.IsPressed())
        {
            currentLock = AxisLock.None;
            return;
        }

        Vector2 delta = input.Camera.Orbit.ReadValue<Vector2>();

        // Trava de Intenção: Decide o eixo nos primeiros milímetros do arraste
        if (currentLock == AxisLock.None && delta.sqrMagnitude > 0.1f)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                currentLock = AxisLock.Horizontal;
            else
                currentLock = AxisLock.Vertical;
        }

        // Soma os valores de rotação APENAS nas nossas variáveis numéricas
        if (currentLock == AxisLock.Horizontal)
        {
            currentRotY += -delta.x * orbitSpeed;
        }
        else if (currentLock == AxisLock.Vertical)
        {
            if (!lockRotationY)
            {
                currentRotX += delta.y * orbitSpeed;
            }
        }

        // A MÁGICA: Constrói uma rotação nova a cada frame. 
        // O Z será literalmente impossível de sair do zero!
        modelRoot.rotation = Quaternion.Euler(currentRotX, currentRotY, 0f);
    }

    private void HandlePan()
    {
        if (thirdPersonFollow == null || !input.Camera.PanButton.IsPressed()) return;

        Vector2 delta = input.Camera.Pan.ReadValue<Vector2>();

        // Se invertPan for verdadeiro, a direção será -1 (invertendo X e Y). Senão, será 1 (normal).
        float direction = invertPan ? -1f : 1f;

        float panX = delta.x * panSpeed * direction;
        float panY = delta.y * panSpeed * direction;

        // Calcula a nova posição desejada
        Vector3 newOffset = thirdPersonFollow.ShoulderOffset + new Vector3(panX, panY, 0f);

        // Calcula os limites reais baseados no ponto de partida
        float minX = initialShoulderOffset.x - maxPanX;
        float maxX = initialShoulderOffset.x + maxPanX;
        float minY = initialShoulderOffset.y - panLimitDown;
        float maxY = initialShoulderOffset.y + panLimitUp;

        // Trava os valores de X e Y dentro desses limites calculados
        newOffset.x = Mathf.Clamp(newOffset.x, minX, maxX);
        newOffset.y = Mathf.Clamp(newOffset.y, minY, maxY);

        // Aplica na câmera
        thirdPersonFollow.ShoulderOffset = newOffset;
    }

    private void HandleZoom()
    {
        if (thirdPersonFollow == null) return;

        float scroll = input.Camera.Zoom.ReadValue<float>();
        if (Mathf.Abs(scroll) < 0.01f) return;

        float scrollDirection = Mathf.Sign(scroll);
        currentDistance -= scrollDirection * zoomSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minZoom, maxZoom);

        thirdPersonFollow.CameraDistance = currentDistance;
    }

    private void ResetView()
    {
        if (cameraPivot != null) cameraPivot.position = initialPivotPosition;
        if (modelRoot != null) modelRoot.localRotation = initialModelRotation;

        if (thirdPersonFollow != null)
        {
            currentDistance = 2f;
            thirdPersonFollow.CameraDistance = currentDistance;
            // Reseta o Pan da tela
            thirdPersonFollow.ShoulderOffset = initialShoulderOffset;
        }
    }
}
