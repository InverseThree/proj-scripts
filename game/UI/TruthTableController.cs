using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class TruthTableController : MonoBehaviour
{
    public Button truthTableButton;

    private GameObject table;
    private GameObject layout;
    private GameObject popup;

    private FloorManager floorManager;

    private int npcCount;

    private void Awake()
    {
        popup = GameObject.FindGameObjectWithTag("popup");

        truthTableButton.onClick.AddListener(SetTable);
    }

    private void Start()
    {
        table = GameObject.FindGameObjectWithTag("truthTable");
        layout = GameObject.FindGameObjectWithTag("tableLayout");

        floorManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<FloorManager>();

        npcCount = floorManager.GetNPCCount();

        switch (npcCount)
        {
            case 2:
                layout.transform.Find("2Variables").gameObject.SetActive(true);
                break;
            case 3:
                layout.transform.Find("3Variables").gameObject.SetActive(true);
                break;
            case 4:
                layout.transform.Find("4Variables").gameObject.SetActive(true);
                break;
            default:
                break;
        }

        table.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !table.activeSelf && !popup.activeSelf && !SayDialog.GetSayDialog().isActiveAndEnabled && !MenuDialog.GetMenuDialog().isActiveAndEnabled && npcCount < 5)
            table.SetActive(true);
        else if (Input.GetKeyDown(KeyCode.T) && table.activeSelf)
            table.SetActive(false);

        if (SayDialog.GetSayDialog().isActiveAndEnabled || MenuDialog.GetMenuDialog().isActiveAndEnabled || npcCount > 4)
            truthTableButton.interactable = false;
        else
            truthTableButton.interactable = true;
    }

    private void SetTable()
    {
        table.SetActive(!table.activeSelf);
    }
}
