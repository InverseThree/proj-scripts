using UnityEngine;
using TMPro;

public class TooltipController : MonoBehaviour
{
    public static TooltipController Instance;

    public RectTransform canvas;

    public GameObject root;
    private RectTransform position;
    public RectTransform background;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;

    private void Awake()
    {
        position = root.transform.GetComponent<RectTransform>();

        Instance = this;
        Hide();
    }

    private void Update()
    {
        Vector2 tooltipPosition  = Input.mousePosition / canvas.localScale.x;

        if (tooltipPosition.x + background.rect.width > canvas.rect.width)
            tooltipPosition.x = canvas.rect.width - background.rect.width;
        if (tooltipPosition.y + background.rect.height > canvas.rect.height)
            tooltipPosition.y = canvas.rect.height - background.rect.height;

        if (root.activeSelf)
            position.anchoredPosition = tooltipPosition;

        Vector2 textSize = bodyText.GetRenderedValues(false);
        Vector2 padding = new Vector2(100, 100);
        background.sizeDelta = textSize + padding;
    }

    public void Show(string title, string body)
    {
        titleText.text = title;
        bodyText.text = body;

        titleText.ForceMeshUpdate();
        bodyText.ForceMeshUpdate();

        root.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}
