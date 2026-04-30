using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TruthTableController : MonoBehaviour
{
    private GameObject table;
    private GameObject layout;
    private GameObject popup;

    private FloorManager floorManager;

    private int npcCount;

    private void Awake()
    {
        popup = GameObject.FindGameObjectWithTag("popup");
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

        if (Input.GetKeyDown(KeyCode.Escape) && table.activeSelf)
            table.SetActive(false);
    }
}
