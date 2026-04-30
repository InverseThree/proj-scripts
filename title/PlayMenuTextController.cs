using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayMenuTextController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI text;
    private Toggle toggle;

    public Color normalColor = new Color(0f, 0f, 0f);
    public Color hoverColor = new Color(0.7981121f, 0.1522039f, 0.1844671f);
    public Color pressedColor = new Color(0.6039216f, 0f, 0.3176471f);
    public Color disabledColor = new Color(0f, 0f, 0f, 0.25f);

    private bool isHovered;
    private bool isPressed;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
        text = GetComponentInChildren<TextMeshProUGUI>();

        if (!toggle.interactable)
            ApplyColor(disabledColor);
        else
            ApplyColor(normalColor);
    }

    private void Update()
    {
        if (!toggle.interactable)
            return;
        else if (toggle.isOn)
            ApplyColor(pressedColor);
        else if (isHovered)
            ApplyColor(hoverColor);
        else
            ApplyColor(normalColor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
    
    private void ApplyColor(Color c)
    {
        if (text != null)
            text.color = c;
    }
}
