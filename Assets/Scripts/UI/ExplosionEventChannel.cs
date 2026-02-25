using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ExplosionEventChannel : MonoBehaviour
{
    private float currentExplosionValue = 0f;
    private Tween explosionTween;

    public static UnityEvent<float> OnExplosionChanged = new UnityEvent<float>();

    void Raise(float value) => OnExplosionChanged?.Invoke(value);

    public void OnSliderChanged(float value) => Raise(value);

    public void ChangeView()
    {
        float target = currentExplosionValue < 0.5f ? 1f : 0f;
        AnimateExplosion(target, 0.6f);
    }

    public void AnimateExplosion(float targetValue, float duration = 0.6f)
    {
        // Se já estiver animando, mata a anterior
        explosionTween?.Kill();

        explosionTween = DOTween
            .To(
                () => currentExplosionValue,
                x =>
                {
                    currentExplosionValue = x;
                    Raise(currentExplosionValue);
                },
                targetValue,
                duration
            )
            .SetEase(Ease.InOutCubic);
    }
}
