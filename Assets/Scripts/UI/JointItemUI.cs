using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JointItemUI : MonoBehaviour
{
    JointCarouselUI controller;

    [SerializeField] Button button;
    [SerializeField] Image thumbnailImage;
    [SerializeField] TMP_Text nameText;
    [SerializeField] GameObject selectionHighlight;

    private JointData data;
    private JointCarouselUI carousel;

    private void Awake()
    {
        controller = GetComponentInParent<JointCarouselUI>();
        button.onClick.AddListener(OnClick);
    }

    private void Start()
    {
        controller.OnJointSelected.AddListener(OnSomeJointSelected);
    }

    private void OnSomeJointSelected(JointItemUI arg0)
    {
        bool status = arg0 == this;
        selectionHighlight.SetActive(status);
    }

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
