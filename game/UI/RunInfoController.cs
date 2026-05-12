using UnityEngine;
using UnityEngine.UI;
using Fungus;
using TMPro;

public class RunInfoController : MonoBehaviour
{
    public GameObject runInfo;
    public GameObject healthBar;
    public TextMeshProUGUI floorCount;
    public Button menuButton;
    public InventorySlotController itemSlot;
    public InventorySlotController relicSlot;

    private GameManager gameManager;
    private FloorManager floorManager;

    private void Start()
    {
        runInfo = GameObject.FindGameObjectWithTag("runInfo");
        healthBar = runInfo.transform.Find("healthBar").gameObject;
        floorCount = runInfo.transform.Find("floor").Find("no.").GetComponent<TextMeshProUGUI>();
        menuButton = runInfo.transform.Find("menuButton").GetComponent<Button>();
        floorManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<FloorManager>();
        gameManager = FindObjectOfType<GameManager>();

        RefreshAll();
    }

    private void Update()
    {
        menuButton.interactable = SayDialog.GetSayDialog().isActiveAndEnabled ? false : (MenuDialog.GetMenuDialog().isActiveAndEnabled ? false : true);
    }

    public void RefreshAll()
    {
        if (gameManager == null || floorManager == null)
            return;

        floorCount.text = floorManager.GetCurrentFloor().ToString();

        for (int i = 0; i <= 3; i++)
        {
            Transform bar = healthBar.transform.Find($"{i}Health");
            if (bar != null)
                bar.gameObject.SetActive(i == gameManager.currentHealth);
        }

        if (itemSlot != null)
            itemSlot.Refresh();
        if (relicSlot != null)
            relicSlot.Refresh();
    }

    public void SetBarrier(bool state)
    {
        healthBar.transform.Find("barrier").gameObject.SetActive(state);
    }
}
