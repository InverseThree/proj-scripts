using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ScythePanelController : MonoBehaviour
{
    public GameObject root;
    public Toggle[] toggles;
    public TextMeshProUGUI info;
    public TextMeshProUGUI[] labels;
    public EventTrigger[] triggers;
    public Button confirmButton;
    public Button cancelButton;
    public GameObject[] npcSpawnPoints;

    public FloorManager floorManager;

    private PuzzleData currentPuzzle;

    private Action<List<int>> callback;

    private int killLeft = 0;

    private void Start()
    {
        root.SetActive(false);

        floorManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<FloorManager>();
        currentPuzzle = GameManager.Instance.currentPuzzle;

        for (int i = 0; i < toggles.Length; i++)
            toggles[i].onValueChanged.AddListener(delegate { SetConfirmButton(); });
    }

    public void Show(Action<List<int>> onConfirm)
    {
        callback = onConfirm;
        root.SetActive(true);

        killLeft = currentPuzzle.npcCount - currentPuzzle.floorState.scytheKillCount;

        info.text = $"You can still KILL {killLeft} inhabitants on this floor";

        for (int i = 0; i < floorManager.spawnedNPCs.Count; i++)
        {
            Debug.Log(floorManager.spawnedNPCs[i].npcSpawnedIndex);

            for (int j = 0; j < toggles.Length; j++)
            {
                if (floorManager.spawnedNPCs[i].npcSpawnedIndex == j)
                {
                    npcSpawnPoints[j].SetActive(true);

                    toggles[j].isOn = false;
                    labels[j].text = floorManager.spawnedNPCs[i].npcName;

                    int npcIndex = i;

                    var enter = triggers[j].triggers.Find(e => e.eventID == EventTriggerType.PointerEnter);
                    var exit  = triggers[j].triggers.Find(e => e.eventID == EventTriggerType.PointerExit);

                    enter.callback.AddListener(d => OnEnter(npcIndex, (PointerEventData)d));
                    exit.callback.AddListener(d => OnExit((PointerEventData)d));
                    break;
                }
            }
        }

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(Confirm);
        confirmButton.interactable = false;

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() => root.SetActive(false));
    }

    private void OnEnter(int index, PointerEventData data)
    {
        TooltipController.Instance.Show(floorManager.spawnedNPCs[index].npcName, GameManager.Instance.currentPuzzle.npcInfo[index].discovered == true ? floorManager.spawnedNPCs[index].statementText : "???");
    }

    private void OnExit(PointerEventData data)
    {
        TooltipController.Instance.Hide();
    }

    private void Confirm()
    {
        List<int> selected = new List<int>();
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].gameObject.activeSelf && toggles[i].isOn)
                selected.Add(i);

            toggles[i].isOn = false;
        }

        callback?.Invoke(selected);
        root.SetActive(false);
    }

    private void SetConfirmButton()
    {
        int toggleOnCount = 0;

        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
                toggleOnCount++;
        }

        if (killLeft < toggleOnCount)
            confirmButton.interactable = false;
        else
        {
            for (int i = 0; i < toggles.Length; i++)
            {
                if (toggles[i].isOn)
                {
                    confirmButton.interactable = true;
                    return;
                }
            }

            confirmButton.interactable = false;
        }
    }
}
