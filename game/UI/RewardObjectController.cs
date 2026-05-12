using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class RewardObjectController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI titleText;
    public Image icon;
    public Image outline;
    public GameObject selectedOutline;

    private RewardPanelController owner;
    private int index;
    private bool isItem;
    private ItemType item;
    private RelicType relic;

    public void SetupItem(RewardPanelController panel, int objIndex, ItemType value)
    {
        owner = panel;
        index = objIndex;
        isItem = true;
        item = value;


        titleText.text = RewardTextLibrary.GetItemName(value);
        icon.sprite = RewardSpriteLibrary.Instance.GetItemSprite(value);
        outline.sprite = RewardSpriteLibrary.Instance.GetItemSprite(value);
        selectedOutline.SetActive(false);
    }

    public void SetupRelic(RewardPanelController panel, int objIndex, RelicType value)
    {
        owner = panel;
        index = objIndex;
        isItem = false;
        relic = value;

        titleText.text = RewardTextLibrary.GetRelicName(value);
        icon.sprite = RewardSpriteLibrary.Instance.GetRelicSprite(value);
        outline.sprite = RewardSpriteLibrary.Instance.GetRelicSprite(value);
        selectedOutline.SetActive(false);
    }

    public void SetSelected(bool selected)
    {
        selectedOutline.SetActive(selected);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        owner.SelectReward(index);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isItem)
            TooltipController.Instance.Show(RewardTextLibrary.GetItemName(item), RewardTextLibrary.GetItemDescription(item));
        else
            TooltipController.Instance.Show(RewardTextLibrary.GetRelicName(relic), RewardTextLibrary.GetRelicDescription(relic));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipController.Instance.Hide();
    }
}
