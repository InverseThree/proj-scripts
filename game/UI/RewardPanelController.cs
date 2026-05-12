using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fungus;

public class RewardPanelController : MonoBehaviour
{
    public GameObject root;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI warningText;
    public FloorManager floorManager;

    public RewardObjectController[] rewards;

    public Button confirmButton;
    public Button discardButton;
    public Button takeButton;
    public Button takeAndUseButton;
    public Button returnButton;

    private List<ItemType> currentItemChoices;
    private List<RelicType> currentRelicChoices;
    private int selectedIndex = -1;

    private Action<ItemType> itemChoiceCallback;
    private Action<RelicType> relicChoiceCallback;
    private Action<ItemOptions> itemOptionsCallback;
    private Action<bool> yesNoCallback;

    private GameObject popup;

    private void Awake()
    {
        popup = GameObject.FindGameObjectWithTag("popup");
    }

    private void Start()
    {
        floorManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<FloorManager>();

        Hide();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !root.activeSelf && !popup.activeSelf && !SayDialog.GetSayDialog().isActiveAndEnabled && !MenuDialog.GetMenuDialog().isActiveAndEnabled && GameManager.Instance.heldItem != ItemType.None)
            ShowUseItemPrompt(GameManager.Instance.heldItem, confirmed =>
            {
                if (confirmed)
                    floorManager.TryUseHeldItem();
            });

        else if (Input.GetKeyDown(KeyCode.I) && root.activeSelf)
            Hide();
    }

    public void Hide()
    {
        root.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        discardButton.gameObject.SetActive(false);
        takeButton.gameObject.SetActive(false);
        takeAndUseButton.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(false);

        for (int i = 0; i < rewards.Length; i++)
            rewards[i].gameObject.transform.parent.gameObject.SetActive(false);

        warningText.text = "";
        selectedIndex = -1;
        currentItemChoices = null;
        currentRelicChoices = null;
    }

    public void ShowMessage(string title, string message)
    {
        root.SetActive(true);
        titleText.text = title;
        warningText.text = message;

        returnButton.gameObject.SetActive(true);
        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(Hide);
    }

    public void ShowUseItemPrompt(ItemType item, Action<bool> callback)
    {
        root.SetActive(true);

        titleText.text = RewardTextLibrary.GetItemName(item);

        if (item == ItemType.Tonic && GameManager.Instance.currentHealth == GameManager.Instance.currentMaxHealth)
        {
            takeButton.interactable = false;
            warningText.text = $"{RewardTextLibrary.GetItemDescription(item)}\n Unusable as you are at full health.";
        }
        else
        {
            takeButton.interactable = true;
            warningText.text = RewardTextLibrary.GetItemDescription(item);
        }

        yesNoCallback = callback;

        takeButton.gameObject.SetActive(true);
        discardButton.gameObject.SetActive(true);

        takeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Use";
        discardButton.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";

        takeButton.onClick.RemoveAllListeners();
        discardButton.onClick.RemoveAllListeners();

        takeButton.onClick.AddListener(() =>
        {
            yesNoCallback?.Invoke(true);
            Hide();
        });

        discardButton.onClick.AddListener(() =>
        {
            yesNoCallback?.Invoke(false);
            Hide();
        });
    }

    public void ShowItemOffer(ItemType item, bool canTake, bool slotFull, Action<ItemOptions> callback)
    {
        root.SetActive(true);
        titleText.text = "Item Obtained";
        warningText.text = canTake ? (slotFull ? "Warning: This will replace your current item." : "") : "Scythe of Origination removed your item slot. You can only discard this item.";

        takeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Take";
        discardButton.GetComponentInChildren<TextMeshProUGUI>().text = "Discard";

        itemOptionsCallback = callback;

        rewards[0].gameObject.transform.parent.gameObject.SetActive(true);
        rewards[0].SetupItem(this, 0, item);

        discardButton.gameObject.SetActive(true);
        discardButton.onClick.RemoveAllListeners();
        discardButton.onClick.AddListener(() =>
        {
            itemOptionsCallback?.Invoke(ItemOptions.Discard);
            Hide();
        });

        if (canTake)
        {
            takeButton.interactable = true;
            takeButton.gameObject.SetActive(true);
            takeButton.onClick.RemoveAllListeners();
            takeButton.onClick.AddListener(() =>
            {
                itemOptionsCallback?.Invoke(ItemOptions.Take);
                Hide();
            });

            if ((item == ItemType.Tonic && GameManager.Instance.currentHealth != GameManager.Instance.currentMaxHealth) || item == ItemType.Mirror)
            {
                takeAndUseButton.gameObject.SetActive(true);
                takeAndUseButton.onClick.RemoveAllListeners();
                takeAndUseButton.onClick.AddListener(() =>
                {
                    itemOptionsCallback?.Invoke(ItemOptions.TakeAndUse);
                    Hide();
                });
            }
        }
    }

    public void ShowItemChoice(List<ItemType> items, bool slotFull, Action<ItemType> callback)
    {
        root.SetActive(true);
        titleText.text = "Choose 1 Item";
        warningText.text = slotFull ? "Warning: This will replace your current item." : "";

        takeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Take";
        discardButton.GetComponentInChildren<TextMeshProUGUI>().text = "Discard";

        discardButton.onClick.RemoveAllListeners();
        discardButton.onClick.AddListener(() =>
        {
            itemOptionsCallback?.Invoke(ItemOptions.Discard);
            Hide();
        });

        currentItemChoices = items;
        itemChoiceCallback = callback;
        selectedIndex = -1;

        for (int i = 0; i < rewards.Length; i++)
        {
            bool active = i < items.Count;
            rewards[i].gameObject.transform.parent.gameObject.SetActive(true);
            if (active)
                rewards[i].SetupItem(this, i, items[i]);
        }

        takeButton.gameObject.SetActive(true);
        takeButton.interactable = false;
        takeButton.onClick.RemoveAllListeners();
        takeButton.onClick.AddListener(() =>
        {
            if (selectedIndex >= 0)
            {
                itemChoiceCallback?.Invoke(currentItemChoices[selectedIndex]);
                Hide();
            }
        });

        discardButton.gameObject.SetActive(true);
    }

    public void ShowRelicChoice(List<RelicType> relics, Action<RelicType> callback)
    {
        root.SetActive(true);
        titleText.text = "Choose 1 Relic";
        warningText.text = "";

        currentRelicChoices = relics;
        relicChoiceCallback = callback;
        selectedIndex = -1;

        for (int i = 0; i < rewards.Length; i++)
        {
            bool active = i < relics.Count;
            rewards[i].gameObject.transform.parent.gameObject.SetActive(true);
            if (active)
                rewards[i].SetupRelic(this, i, relics[i]);
        }

        confirmButton.gameObject.SetActive(true);
        confirmButton.interactable = false;
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            if (selectedIndex >= 0)
            {
                relicChoiceCallback?.Invoke(currentRelicChoices[selectedIndex]);
                Hide();
            }
        });
    }

    public void SelectReward(int index)
    {
        selectedIndex = index;

        for (int i = 0; i < rewards.Length; i++)
            if (rewards[i].gameObject.activeSelf)
                rewards[i].SetSelected(i == index);

        if (confirmButton.gameObject.activeSelf)
            confirmButton.interactable = true;
        
        if (takeButton.gameObject.activeSelf)
            takeButton.interactable = true;
    }
}
