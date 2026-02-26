using UnityEngine;
using UnityEngine.UI;

public class SlideValue : MonoBehaviour
{
    [SerializeField] ExplosionUIController controller;

    Slider slider;

    private void Awake()
    {
        controller = GetComponentInParent<ExplosionUIController>();
        slider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        if(!slider || !controller)
            return;

        var value = controller.CurrentValue;
        slider.value = value;
    }
}
