using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using TMPro;

public class RunInfoController : MonoBehaviour
{
    private GameObject runInfo;
    private GameObject healthBar;
    private TextMeshProUGUI floorCount;
    private Button menuButton;

    private GameManager gameManager;
    private FloorManager floorManager;

    private int previousHealth;

    private void Start()
    {
        runInfo = GameObject.FindGameObjectWithTag("runInfo");
        healthBar = runInfo.transform.Find("healthBar").gameObject;
        floorCount = runInfo.transform.Find("floor").gameObject.transform.Find("no.").gameObject.GetComponent<TextMeshProUGUI>();
        menuButton = runInfo.transform.Find("menuButton").gameObject.GetComponent<Button>();
        floorManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<FloorManager>();
        gameManager = FindObjectOfType<GameManager>();

        floorCount.text = floorManager.GetCurrentFloor().ToString();

        previousHealth = gameManager.currentHealth;

        healthBar.transform.Find($"{previousHealth}Health").gameObject.SetActive(true);
    }

    private void Update()
    {
        if (SayDialog.GetSayDialog().isActiveAndEnabled || MenuDialog.GetMenuDialog().isActiveAndEnabled)
            menuButton.interactable = false;
        else
            menuButton.interactable = true;
    }

    public void ChangeHealth(bool lose)
    {
        healthBar.transform.Find($"{previousHealth}Health").gameObject.SetActive(false);

        healthBar.transform.Find($"{gameManager.currentHealth}Health").gameObject.SetActive(true);
    }
}
