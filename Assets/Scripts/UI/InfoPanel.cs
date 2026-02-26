using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI textBox;
    Vector2 closedPosition;

    [Header("Animation Setup")]
    [SerializeField] Vector2 openedPosition;
    [SerializeField] float slideDuration = 0.4f;

    Tween slideTween;

    private void Awake()
    {
        if (button && textBox)
            return;

        button = GetComponentInChildren<Button>();
        textBox = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        button.onClick.AddListener(OnClick);
        closedPosition = transform.localPosition;
    }

    private void OnClick()
    {
        //TODO resgatar os dados da joint atual

        bool isOpened = transform.localPosition.x != closedPosition.x;
        var nextPosition = isOpened ? closedPosition : openedPosition;

        slideTween?.Kill();

        slideTween = transform
            .DOLocalMove(nextPosition, slideDuration)
            .SetEase(Ease.OutCubic);
    }
}
