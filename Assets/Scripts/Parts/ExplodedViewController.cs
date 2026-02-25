using UnityEngine;


public class ExplodedViewController : MonoBehaviour
{
    public float explosionDistance = 0.3f;

    private ExplodablePart[] parts;

    private void OnEnable()
    {
        ExplosionEventChannel.OnExplosionChanged.AddListener(SetExplosion);
    }

    private void OnDisable()
    {
        ExplosionEventChannel.OnExplosionChanged.RemoveListener(SetExplosion);
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
}
