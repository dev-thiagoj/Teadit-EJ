using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public class ExplodedViewController : MonoBehaviour
{
    public float explosionDistance = 0.3f;

    [SerializeField] ExplodablePart[] parts;

    private void OnEnable()
    {
        ExplosionUIController.OnExplosionChanged.AddListener(SetExplosion);
    }

    private void OnDisable()
    {
        ExplosionUIController.OnExplosionChanged.RemoveListener(SetExplosion);
    }

    void Awake()
    {
        parts = GetComponentsInChildren<ExplodablePart>();
    }

    public void SetExplosion(float value)
    {
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i].SetExplosion(value, explosionDistance);
        }
    }

    [ContextMenu("Get Parts")]
    void GetParts()
    {
        parts = GetComponentsInChildren<ExplodablePart>();
    }

    [ContextMenu("GetPartsPosition")]
    void GetPartsPosition()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i].GetInitialPosition();
        }
    }

    [ContextMenu("Explode")]
    void Explode() => SetExplosion(1);

    [ContextMenu("Solid")]
    void Solid() => SetExplosion(0);
}
