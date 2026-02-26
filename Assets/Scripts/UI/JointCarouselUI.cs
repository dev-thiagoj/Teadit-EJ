using UnityEngine;

public class JointCarouselUI : MonoBehaviour
{
    public JointDatabase database;

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
        // Limpa o conteúdo antes de gerar (boa prática)
        foreach (Transform child in contentParent) Destroy(child.gameObject);

        // Lemos os dados diretamente do Database
        if (database == null || database.Joints == null) return;

        foreach (var joint in database.Joints)
        {
            JointItemUI item = Instantiate(itemPrefab, contentParent);
            item.Setup(joint, this);
        }

        if (contentParent.childCount > 0)
        {
            contentParent.GetChild(0).GetComponent<JointItemUI>().OnClick();
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
