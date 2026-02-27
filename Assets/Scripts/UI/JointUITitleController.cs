using System;
using TMPro;
using UnityEngine;

public class JointUITitleController : MonoBehaviour
{
    [SerializeField] JointManager jointManager;

    TextMeshProUGUI titleBox;

    private void Awake()
    {
        titleBox = GetComponentInChildren<TextMeshProUGUI>();
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
}
