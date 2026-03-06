using BMV.Dialogs;
using System;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [Header("Referencias")]
    [SerializeField] JointUITitleController header;
    [SerializeField] Textbox textbox;
    //[SerializeField] InfoPanel infoPanel;

    private void OnDestroy()
    {
        Textbox.OnClosed -= OnDialogueEnds;
        textbox.PrepareToOpen -= Hide;
    }

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(instance);

        if (header)
            return;

        header = GetComponentInChildren<JointUITitleController>();

        textbox = GetComponentInChildren<Textbox>();
        Textbox.OnClosed += OnDialogueEnds;
        textbox.PrepareToOpen += Hide;
    }

    private void Start()
    {
        header.InstantHide();
        textbox.Open();
    }

    private void OnDialogueEnds()
    {
        header.Show();
    }

    void Hide() => header.Hide();

    //#E6E6E6
}
