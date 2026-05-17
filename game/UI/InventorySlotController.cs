using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Fungus;

public class InventorySlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public enum SlotType
    {
        Item, 
        Relic
    }

    public Button root;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Fungus;

public class InventorySlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public enum SlotType
    {
        Item, 
        Relic
    }

    public Button root;
    public Button iconButton;
    public SlotType slotType;
    public Image iconImage;
    public GameObject emptyMarker;
    public GameObject disableMarker;

    private void Update()
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Fungus;

public class InventorySlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public enum SlotType
    {
        Item, 
        Relic
    }

    public Button root;
    public Button iconButton;
    public SlotType slotType;
    public Image iconImage;
    public GameObject emptyMarker;
    public GameObject disableMarker;

    private void Update()
    {
        if (SayDialog.GetSayDialog().isActiveAndEnabled || MenuDialog.GetMenuDialog().isActiveAndEnabled)
        {
            root.interactable = false;
            iconButton.interactable = false;
        }
        else
        {
            root.interactable = true;
            iconButton.interactable = true;
        }
    }

    public void Refresh()
    {
        if (slotType == SlotType.Item)
        {
            ItemType item = GameManager.Instance.heldItem;
            bool hasItem = item != ItemType.None;
            bool scytheActive = GameManager.Instance.heldRelic == RelicType.Scythe;

            emptyMarker.SetActive(!hasItem);
            disableMarker.SetActive(scytheActive);
            iconImage.enabled = hasItem;

            if (hasItem)
                iconImage.sprite = RewardSpriteLibrary.Instance.GetItemSprite(item);
        }
        else
        {
            RelicType relic = GameManager.Instance.heldRelic;
            bool hasRelic = relic != RelicType.None;

            emptyMarker.SetActive(!hasRelic);
            iconImage.enabled = hasRelic;

            if (hasRelic)
                iconImage.sprite = RewardSpriteLibrary.Instance.GetRelicSprite(relic);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if (!SayDialog.GetSayDialog().isActiveAndEnabled && !MenuDialog.GetMenuDialog().isActiveAndEnabled)
        {
            if (slotType == SlotType.Item)
            {
                if (GameManager.Instance.heldItem != ItemType.None)
                    TooltipController.Instance.Show(RewardTextLibrary.GetItemName(GameManager.Instance.heldItem), RewardTextLibrary.GetItemDescription(GameManager.Instance.heldItem));
            }
            else
            {
                if (GameManager.Instance.heldRelic != RelicType.None)
                    TooltipController.Instance.Show(RewardTextLibrary.GetRelicName(GameManager.Instance.heldRelic), RewardTextLibrary.GetRelicDescription(GameManager.Instance.heldRelic));
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipController.Instance.Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FloorManager floorManager = FindObjectOfType<FloorManager>();
        if (floorManager == null) return;


        if (!SayDialog.GetSayDialog().isActiveAndEnabled && !MenuDialog.GetMenuDialog().isActiveAndEnabled)
        {
            if (slotType == SlotType.Item)
                floorManager.OnItemSlotClicked();
            else
                floorManager.OnRelicSlotClicked();
        }
    }
}
