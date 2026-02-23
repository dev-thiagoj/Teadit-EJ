using UnityEngine;


public class ExplodablePart : MonoBehaviour
{
    public Vector3 localDirection = Vector3.right; // definir no Inspector
    public float distanceMultiplier = 1f; // opcional (cada peça pode ir mais longe)

    private Vector3 initialLocalPosition;

    void Awake()
    {
        initialLocalPosition = transform.localPosition;

        // garante que a direção seja normalizada
        localDirection = localDirection.normalized;
    }

    public void SetExplosion(float value, float globalDistance)
    {
        transform.localPosition =
            initialLocalPosition +
            localDirection * globalDistance * distanceMultiplier * value;
    }
}
