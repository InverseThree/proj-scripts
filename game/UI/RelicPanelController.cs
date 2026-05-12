using System;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class RelicPanelController : MonoBehaviour
{
    public GameObject root;
    public FloorManager floorManager;
    public GameObject defaultPanel;
    public GameObject scythePanel;
    public GameObject lampPanel;

    private GameObject popup;

    private void Awake()
    {
        popup = GameObject.FindGameObjectWithTag("popup");
    }

    private void Start()
    {
        floorManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<FloorManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !popup.activeSelf && !defaultPanel.activeSelf && !scythePanel.activeSelf && !lampPanel.activeSelf && !SayDialog.GetSayDialog().isActiveAndEnabled && !MenuDialog.GetMenuDialog().isActiveAndEnabled)
            floorManager.OnRelicSlotClicked();
        else if (Input.GetKeyDown(KeyCode.R) && (defaultPanel.activeSelf || scythePanel.activeSelf || lampPanel.activeSelf))
        {
            defaultPanel.SetActive(false);
            scythePanel.SetActive(false);
            lampPanel.SetActive(false);
        }
    }
}
