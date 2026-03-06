using DG.Tweening;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class JointManager : MonoBehaviour
{
    public static JointManager Instance;

    [SerializeField] Transform spawnPoint;

    [SerializeField] private CinemachineTargetGroup targetGroup;

    private GameObject currentJoint;

    public UnityEvent<JointContext> OnJointLoaded;

    Tween resetTween;

    JointData currentJointData;
    public JointData CurrentJoint => currentJointData;

    private void OnDisable()
    {
        ExplosionUIController.OnResetViewed.RemoveListener(ResetView);
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);

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
        // Remove anterior
        if (currentJoint != null)
            Destroy(currentJoint);

        // Instancia nova
        currentJoint = Instantiate(
            data.prefab,
            spawnPoint.position,
            Quaternion.identity,
            spawnPoint
        );

        currentJointData = data;

        // Atualiza Target Group
        SetupTargetGroup(currentJoint);

        // Cria contexto (se ainda quiser usar para UI etc)
        JointContext context = new JointContext(currentJoint);
        context.JointName = data.jointName;

        OnJointLoaded?.Invoke(context);
    }

    private void SetupTargetGroup(GameObject jointRoot)
    {
        if (targetGroup == null)
            return;

        // Limpa membros atuais
        targetGroup.Targets.Clear();

        var renderers = jointRoot.GetComponentsInChildren<Renderer>();

        List<CinemachineTargetGroup.Target> newTargets =
            new List<CinemachineTargetGroup.Target>();

        foreach (var r in renderers)
        {
            CinemachineTargetGroup.Target t =
                new CinemachineTargetGroup.Target
                {
                    Object = r.transform,
                    Weight = 1f,
                    Radius = r.bounds.extents.magnitude
                };

            newTargets.Add(t);
        }

        targetGroup.Targets = newTargets;
    }
}


