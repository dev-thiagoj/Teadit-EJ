using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Button closeBtn;

    private void OnDestroy()
    {
        closeBtn.onClick.RemoveListener(Quit);
    }

    private void Start()
    {
        closeBtn.onClick.AddListener(Quit);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }
#endif
        Application.Quit();
    }
}
