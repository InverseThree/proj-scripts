using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScythePanelController : MonoBehaviour
{
    public GameObject root;
    public Toggle[] toggles;
    public TextMeshProUGUI[] labels;
    public Button confirmButton;
    public Button cancelButton;

    public RectTransform[] npcSpawnPoints;
    private readonly List<NPCController> spawnedNpcs;

    private Action<List<int>> callback;

    private void Start()
    {
        root.SetActive(false);
    }

    public void Show(int npcCount, string[] npcLabels, Action<List<int>> onConfirm)
    {
        callback = onConfirm;
        root.SetActive(true);

        for (int i = 0; i < toggles.Length; i++)
        {
            bool active = i < npcCount;
            toggles[i].gameObject.SetActive(active);
            labels[i].gameObject.SetActive(active);

            if (active)
            {
                toggles[i].isOn = false;
                labels[i].text = npcLabels[i];
            }
        }

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(Confirm);

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() => root.SetActive(false));
    }

    private void Confirm()
    {
        List<int> selected = new List<int>();
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].gameObject.activeSelf && toggles[i].isOn)
                selected.Add(i);
        }

        callback?.Invoke(selected);
        root.SetActive(false);
    }
}
