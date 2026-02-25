using System.Collections.Generic;
using UnityEngine;

public class JointCarouselUI : MonoBehaviour
{
    public List<JointData> joints;

    public JointItemUI itemPrefab;
    public Transform contentParent;

    public JointManager jointManager;

    private JointItemUI currentSelected;

    void Start()
    {
        BuildCarousel();
    }

    void BuildCarousel()
    {
        foreach (var joint in joints)
        {
            JointItemUI item = Instantiate(itemPrefab, contentParent);
            item.Setup(joint, this);
        }

        // Seleciona o primeiro automaticamente
        if (contentParent.childCount > 0)
        {
            contentParent.GetChild(0)
                .GetComponent<JointItemUI>()
                .OnClick();
        }
    }

    public void SelectJoint(JointData data, JointItemUI itemUI)
    {
        if (currentSelected != null)
            currentSelected.SetSelected(false);

        currentSelected = itemUI;
        currentSelected.SetSelected(true);

        jointManager.LoadJoint(data);
    }
}
