using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JointItemUI : MonoBehaviour
{
    public Image thumbnailImage;
    public TMP_Text nameText;
    public GameObject selectionHighlight;

    private JointData data;
    private JointCarouselUI carousel;

    public void Setup(JointData jointData, JointCarouselUI parent)
    {
        data = jointData;
        carousel = parent;

        thumbnailImage.sprite = data.thumbnail;
        nameText.text = data.code;
    }

    public void OnClick()
    {
        carousel.SelectJoint(data, this);
    }

    public void SetSelected(bool value)
    {
        selectionHighlight.SetActive(value);
        transform.localScale = value ? Vector3.one * 1.05f : Vector3.one;
    }
}
