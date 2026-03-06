using BMV.Dialogs;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] Textbox textContainer;
    JointManager jointManager;

    [SerializeField] Button button;
    //[SerializeField] TextMeshProUGUI textBox;
    Vector2 closedPosition;

    [Header("Animation Setup")]
    [SerializeField] Vector2 openedPosition;
    [SerializeField] float slideDuration = 0.4f;

    Tween slideTween;

    private void OnDestroy()
    {
        jointManager.OnJointLoaded.RemoveListener(OnJointLoaded);
    }

    private void Awake()
    {
        //if (button && textBox)
        //    return;

        button = GetComponentInChildren<Button>();
        //textBox = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        button.onClick.AddListener(OnClick);
        closedPosition = transform.localPosition;

        jointManager = JointManager.Instance;
        jointManager.OnJointLoaded.AddListener(OnJointLoaded);
    }

    private void OnJointLoaded(JointContext arg0)
    {
        var currentData = JointManager.Instance.CurrentJoint;

        //if (!currentData)
        //    return;

        //textContainer.SetText(currentData.info.Question);

        //SetInfo(currentData);
    }

    private void OnClick()
    {
        if (JointManager.Instance == null)
            return;

        var currentData = JointManager.Instance.CurrentJoint;

        if (!currentData) 
            return;

        textContainer.SetText(currentData.info.Question);

        //bool isOpened = transform.localPosition.x != closedPosition.x;
        //var nextPosition = isOpened ? closedPosition : openedPosition;

        //slideTween?.Kill();

        //slideTween = transform
        //    .DOLocalMoveX(nextPosition.x, slideDuration)
        //    .SetEase(Ease.OutCubic);
    }

    //void SetInfo(JointData currentData)
    //{
    //    var info = currentData.info.Question;
    //    textBox.text = info;
    //}
}
