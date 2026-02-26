using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExplosionUIController : MonoBehaviour
{
    private float currentExplosionValue = 0f;
    private Tween explosionTween;

    Slider slider;

    public float CurrentValue => currentExplosionValue;

    private void Awake()
    {
        if (slider)
            return;

        slider = GetComponentInChildren<Slider>(true);
    }

    public static UnityEvent<float> OnExplosionChanged = new UnityEvent<float>();
    public static UnityEvent OnResetViewed = new();

    void Raise(float value)
    {
        currentExplosionValue = value;
        OnExplosionChanged?.Invoke(value);
    }

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

    public void ToogleSlider()
    {
        bool isActive = slider.gameObject.activeSelf;
        slider.gameObject.SetActive(!isActive);

        if (slider.gameObject.activeSelf)
            slider.value = currentExplosionValue;
    }

    public void ResetView()
    {
        AnimateExplosion(0);
        OnResetViewed.Invoke();
    }
}
