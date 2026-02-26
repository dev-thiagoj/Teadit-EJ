using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class JointCarouselUI : MonoBehaviour
{
    public JointDatabase database;
    public JointItemUI itemPrefab;
    public Transform contentParent;
    public JointManager jointManager;

    [Header("Configurações de Layout")]
    public float spacing = 40f;
    public float duration = 0.4f;
    public float maskWidth = 1080f;

    [Header("Botões de Navegação")]
    public Button btnNext;
    public Button btnPrevious;

    private int viewIndex = 0;
    private RectTransform contentRect;
    private float itemWidth;
    private JointItemUI currentSelected;

    void Start()
    {
        contentRect = contentParent.GetComponent<RectTransform>();
        itemWidth = itemPrefab.GetComponent<RectTransform>().rect.width;

        // Pivot em 0 garante que o primeiro item comece na esquerda (x=0)
        contentRect.pivot = new Vector2(0, 0.5f);
        contentRect.anchorMin = new Vector2(0, 0.5f);
        contentRect.anchorMax = new Vector2(0, 0.5f);

        BuildCarousel();
    }

    public void BuildCarousel()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        if (database == null || database.Joints == null || database.Joints.Count == 0)
            return;

        foreach (var joint in database.Joints)
        {
            JointItemUI item = Instantiate(itemPrefab, contentParent);
            item.Setup(joint, this);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);

        // Inicia no primeiro item, colado na esquerda
        viewIndex = 0;
        UpdateScrollPosition(true);
    }

    public void Next()
    {
        int totalItems = database.Joints.Count;
        if (totalItems <= 1) return;

        // Só avança se não atingiu o limite da direita
        if (!IsAtEnd())
        {
            viewIndex++;
            UpdateScrollPosition(false);
        }
    }

    public void Previous()
    {
        if (viewIndex > 0)
        {
            viewIndex--;
            UpdateScrollPosition(false);
        }
    }

    private void UpdateScrollPosition(bool immediate)
    {
        float step = itemWidth + spacing;
        float targetX = -(viewIndex * step);

        // Cálculo do limite da direita: largura total do conteúdo menos a largura da máscara
        float contentWidth = (database.Joints.Count * itemWidth) + ((database.Joints.Count - 1) * spacing);
        float maxScroll = Mathf.Max(0, contentWidth - maskWidth);

        // Trava o movimento entre o início (0) e o final máximo (-maxScroll)
        targetX = Mathf.Clamp(targetX, -maxScroll, 0);

        contentRect.DOKill();

        if (immediate)
            contentRect.anchoredPosition = new Vector2(targetX, contentRect.anchoredPosition.y);
        else
            contentRect.DOAnchorPosX(targetX, duration).SetEase(Ease.OutCubic);

        UpdateButtonStates(targetX, maxScroll);
    }

    private void UpdateButtonStates(float currentX, float maxScroll)
    {
        // Desativa Previous se estiver no início (0)
        if (btnPrevious != null)
            btnPrevious.interactable = currentX < -1f; // -1f para evitar erro de precisão float

        // Desativa Next se o final do conteúdo encostou na borda da máscara
        if (btnNext != null)
            btnNext.interactable = currentX > (-maxScroll + 1f);
    }

    private bool IsAtEnd()
    {
        float contentWidth = (database.Joints.Count * itemWidth) + ((database.Joints.Count - 1) * spacing);
        float maxScroll = Mathf.Max(0, contentWidth - maskWidth);
        return contentRect.anchoredPosition.x <= -maxScroll + 1f;
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
