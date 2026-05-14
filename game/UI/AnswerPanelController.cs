using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using TMPro;

public class AnswerPanelController : MonoBehaviour
{
    public GameObject statementSet;
    private GameObject rowsContainer;
    public GameObject popup;
    public Button submitButton;
    public Toggle toggle;
    private TextMeshProUGUI statement;

    private GameObject[] npc;
    private TMP_Dropdown[] answer;
    private int[] guess;

    public FloorManager floorManager;

    private int npcCount;
    private string[] npcLabels = FloorManager.npcLabels;

    private void Awake()
    {
        popup = GameObject.FindGameObjectWithTag("popup");
    }

    private void Start()
    {
        statementSet = GameObject.FindGameObjectWithTag("statementSet");
        submitButton = GameObject.FindGameObjectWithTag("submit").GetComponent<Button>();
        toggle = GameObject.FindGameObjectWithTag("toggle").GetComponent<Toggle>();

        floorManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<FloorManager>();

        npcCount = floorManager.GetNPCCount();

        npc = new GameObject[npcCount];
        answer = new TMP_Dropdown[npcCount];
        guess = new int[npcCount];

        switch (npcCount)
        {
            case 2:
                rowsContainer = statementSet.transform.Find("2Variables").gameObject;
                break;
            case 3:
                rowsContainer = statementSet.transform.Find("3Variables").gameObject;
                break;
            case 4:
                rowsContainer = statementSet.transform.Find("4Variables").gameObject;
                break;
            case 5:
                rowsContainer = statementSet.transform.Find("5Variables").gameObject;
                break;
            case 6:
                rowsContainer = statementSet.transform.Find("6Variables").gameObject;
                break;
            default:
                break;
        }

        for (int i = 0; i < npcCount; i++)
        {
            npc[i] = rowsContainer.transform.Find(npcLabels[i]).gameObject;
            answer[i] = npc[i].transform.Find("answer").gameObject.GetComponent<TMP_Dropdown>();
            answer[i].onValueChanged.AddListener(delegate{ SetSubmitButton(); });

            if (GameManager.Instance.currentPuzzle.PeasantEnabled())
                answer[i].AddOptions(new List<string>{ "Peasant" });
        }

        submitButton.onClick.AddListener(floorManager.SubmitAnswers);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !popup.activeSelf && !SayDialog.GetSayDialog().isActiveAndEnabled && !MenuDialog.GetMenuDialog().isActiveAndEnabled)
        {
            toggle.isOn = !toggle.isOn;
            rowsContainer.SetActive(!rowsContainer.activeSelf);
        }
    }

    private void SetSubmitButton()
    {
        for (int i = 0; i < npcCount; i++)
        {
            if (answer[i].value == 0)
            {
                submitButton.interactable = false;
                return;
            }
        }

        submitButton.interactable = true;
    }

    public void SetStatement(int npcIndex, string statementText)
    {
        statement = npc[npcIndex].transform.Find("statement").gameObject.GetComponent<TextMeshProUGUI>();
        statement.text = statementText;
    }

    public int[] GetGuesses()
    {
        for (int i = 0; i < npcCount; i++)
            guess[i] = answer[i].value;

        return guess;
    }

    public void RevealAll(Role[] role)
    {
        for (int i = 0; i < npcCount; i++)
        {
            answer[i].value = (int) role[i];
            answer[i].interactable = false;
        }

        submitButton.interactable = false;
    }

    public void RevealIdentity(string chosen, Role role)
    {
        for (int i = 0; i < npcCount; i++)
        {
            if (npc[i] == rowsContainer.transform.Find(chosen).gameObject)
            {
                answer[i].value = (int) role;
                answer[i].interactable = false;
            }
        }
    }

    public void BuildFromPuzzle(PuzzleData puzzle)
    {
        for (int i = 0; i < puzzle.npcCount; i++)
        {
            string text = puzzle.npcInfo[i].discovered ? puzzle.npcInfo[i].statementText : "???";
            SetStatement(i, text);

            answer[i].value = puzzle.playerGuesses[i];
            answer[i].interactable = !puzzle.npcInfo[i].identityRevealed;

            if (puzzle.npcInfo[i].identityRevealed)
                answer[i].value = (int)puzzle.role[i];
        }

        SetSubmitButton();
    }
}
