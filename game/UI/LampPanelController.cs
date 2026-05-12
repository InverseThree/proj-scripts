using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LampPanelController : MonoBehaviour
{
    [Serializable]
    public class LampWishRequest
    {
        public LampWishType type;
        public int a;
        public int b;
    }

    public GameObject root;
    public TMP_Dropdown typeDropdown;
    public TMP_Dropdown npcADropdown;
    public TMP_Dropdown npcBDropdown;
    public TextMeshProUGUI remainingText;
    public Button confirmButton;
    public Button cancelButton;

    private Action<LampWishRequest> callback;

    private void Start()
    {
        root.SetActive(false);
    }

    private void Update()
    {
        RunModifierState state = GameManager.Instance.modifierState;

        if (typeDropdown.value == 0)
        {
            npcADropdown.gameObject.SetActive(false);
            npcBDropdown.gameObject.SetActive(false);

            confirmButton.interactable = false;
        }
        else if (typeDropdown.value == 1)
        {
            npcADropdown.gameObject.SetActive(true);
            npcBDropdown.gameObject.SetActive(false);

            if (state.lampIdentityTotalUsed == 1 || state.lampTotalUsed == 3)
                confirmButton.interactable = false;
            else
                confirmButton.interactable = true;
        }
        else if (typeDropdown.value == 2)
        {
            npcADropdown.gameObject.SetActive(true);
            npcBDropdown.gameObject.SetActive(true);

            if (state.lampPairTotalUsed == 2 || state.lampTotalUsed == 3 || npcADropdown.value == npcBDropdown.value)
                confirmButton.interactable = false;
            else
                confirmButton.interactable = true;
        }
        else if (typeDropdown.value == 3)
        {
            npcADropdown.gameObject.SetActive(false);
            npcBDropdown.gameObject.SetActive(false);

            if (state.lampCountTotalUsed == 3 || state.lampTotalUsed == 3)
                confirmButton.interactable = false;
            else
                confirmButton.interactable = true;
        }
    }

    public void Show(int npcCount, string[] npcLabels, RunModifierState state, Action<LampWishRequest> onConfirm)
    {
        callback = onConfirm;
        root.SetActive(true);

        typeDropdown.ClearOptions();
        typeDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "",
            "Reveal a chosen inhabitant's identity on this floor",
            "Reveal whether 2 chosen inhabitants' identity is the same on this floor",
            "Reveal the total number of knights and knaves on this floor"
        });

        npcADropdown.ClearOptions();
        npcBDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        for (int i = 0; i < npcCount; i++)
            options.Add(npcLabels[i]);

        npcADropdown.AddOptions(options);
        npcBDropdown.AddOptions(options);

        remainingText.text =
            $"Total wishes left: {3 - state.lampTotalUsed}\n" +
            $"First ability wish left: {1 - state.lampIdentityTotalUsed}\n" +
            $"Second ability wish left: {2 - state.lampPairTotalUsed}\n" +
            $"Third ability wish left: {3 - state.lampCountTotalUsed}";

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            LampWishRequest request = new LampWishRequest
            {
                type = (LampWishType)typeDropdown.value,
                a = npcADropdown.value,
                b = npcBDropdown.value
            };

            callback?.Invoke(request);
            root.SetActive(false);
        });

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() => root.SetActive(false));
    }
}
