using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;
using Fungus;

public class HintLogController : MonoBehaviour
{
    public Image hintButtonIcon;
    public Button hintButton;
    public GameObject hintPanel;
    public TextMeshProUGUI hintText;

    public Sprite hintOff;
    public Sprite hintOn;

    private GameObject popup;

    private void Awake()
    {
        popup = GameObject.FindGameObjectWithTag("popup");

        hintPanel.SetActive(false);

        hintButton.onClick.AddListener(SetPanel);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && !hintPanel.activeSelf && !popup.activeSelf && !SayDialog.GetSayDialog().isActiveAndEnabled && !MenuDialog.GetMenuDialog().isActiveAndEnabled)
            hintPanel.SetActive(true);
        else if (Input.GetKeyDown(KeyCode.H) && hintPanel.activeSelf)
            hintPanel.SetActive(false);

        if (SayDialog.GetSayDialog().isActiveAndEnabled || MenuDialog.GetMenuDialog().isActiveAndEnabled)
            hintButton.interactable = false;
        else
            hintButton.interactable = true;
    }

    private void SetPanel()
    {
        hintPanel.SetActive(!hintPanel.activeSelf);
    }

    public void BuildFromPuzzle(PuzzleData puzzle)
    {
        if (puzzle == null || puzzle.hints == null || puzzle.hints.Count == 0)
        {
            hintButtonIcon.sprite = hintOff;
            hintText.text = "No known hint on this floor.";
            return;
        }

        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < puzzle.hints.Count; i++)
        {
            stringBuilder.Append("• ");
            stringBuilder.AppendLine(puzzle.hints[i]);
        }

        hintButtonIcon.sprite = hintOn;
        hintText.text = stringBuilder.ToString();
    }
}
