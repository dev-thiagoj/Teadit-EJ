using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class JointManager : MonoBehaviour
{
    public Transform spawnPoint;

    private GameObject currentJoint;

    public UnityEvent<JointContext> OnJointLoaded;

    Tween resetTween;

    private void OnDisable()
    {
        ExplosionUIController.OnResetViewed.RemoveListener(ResetView);
    }

    private void Awake()
    {
        ExplosionUIController.OnResetViewed.AddListener(ResetView);
    }

    private void ResetView()
    {
        if (currentJoint == null)
            return;

        resetTween?.Kill();

        resetTween = currentJoint.transform
                        .DOLocalRotate(Vector3.zero, 0.6f)
                        .SetEase(Ease.OutCubic);
    }

    public void LoadJoint(JointData data)
    {
        if (currentJoint != null)
            Destroy(currentJoint);

        currentJoint = Instantiate(
            data.prefab,
            spawnPoint.position,
            Quaternion.identity,
            spawnPoint
        );

        JointViewReferences refs =
            currentJoint.GetComponent<JointViewReferences>();

        JointContext context = new JointContext(currentJoint);

        // 🔥 adicionamos os pivôs
        context.OrbitPivot = refs.orbitPivot;
        context.PanPivot = refs.panPivot;

        OnJointLoaded?.Invoke(context);

        ResetExplosion();
        ResetCamera();
    }

    void ResetExplosion()
    {
        // Chame seu sistema de explosão aqui
        // Ex: currentJoint.GetComponent<ExplosionController>()?.Reset();
    }

    void ResetCamera()
    {
        // Chame seu sistema de framing aqui
        // Ex: CameraController.Instance.FrameTarget(currentJoint);
    }
}


