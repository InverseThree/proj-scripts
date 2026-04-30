using UnityEngine;
using UnityEngine.UI;

public class ToggleIcon : MonoBehaviour
{
    [Header("Setup")]
    public Toggle toggle;
    public Image icon;

    public Sprite isOnIcon;
    public Sprite isOffIcon;


    private void Reset()
    {
        toggle = GetComponent<Toggle>();
    }

    private void Awake()
    {
        toggle.onValueChanged.AddListener(UpdateIcon);
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(UpdateIcon);
    }

    private void UpdateIcon(bool isExpanded)
    {
        icon.sprite = isExpanded ? isOnIcon : isOffIcon;
    }
}
