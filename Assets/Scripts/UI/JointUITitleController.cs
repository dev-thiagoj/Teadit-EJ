using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class JointUITitleController : MonoBehaviour, ISession
{
    [SerializeField] JointManager jointManager;

    TextMeshProUGUI titleBox;
    CanvasGroup canvasGroup;

    [SerializeField] int indexDecision;
    public int IndexDecision => indexDecision;

    bool isActive;
    public bool IsActive => isActive;

    private void OnDestroy()
    {
        jointManager.OnJointLoaded.RemoveListener(OnJointLoaded);
    }

    private void Awake()
    {
        titleBox = GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        jointManager.OnJointLoaded.AddListener(OnJointLoaded);
    }

    private void OnJointLoaded(JointContext ctx)
    {
        var name = ctx.JointName;
        titleBox.text = name;
    }

    [ContextMenu("Show")]
    public void Show()
    {
        canvasGroup.DOFade(1f, .5f).OnComplete(() =>
        {
            canvasGroup.interactable = true;
        });
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        canvasGroup.DOFade(0f, .5f).OnComplete(() =>
        {
            canvasGroup.interactable = false;
        });
    }

    public void InstantHide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
    }
}
