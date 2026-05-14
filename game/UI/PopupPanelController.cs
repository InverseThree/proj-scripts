using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fungus;
using TMPro;

public class PopupPanelController : MonoBehaviour
{
    public GameObject popup;
    public GameObject panel;
    public GameObject table;
    public GameObject answer;
    public Toggle toggle;

    public FloorManager floorManager;

    private GameObject layout;

    private Button option1;
    private Button option2;
    private Button option3;

    private void Awake()
    {
        table = GameObject.FindGameObjectWithTag("truthTable");
        answer = GameObject.FindGameObjectWithTag("answer");
        toggle = GameObject.FindGameObjectWithTag("toggle").GetComponent<Toggle>();
    }

    private void Start()
    {
        popup = GameObject.FindGameObjectWithTag("popup");
        panel = popup.transform.Find("panel").gameObject;
        floorManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<FloorManager>();

        popup.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !popup.activeSelf && !SayDialog.GetSayDialog().isActiveAndEnabled && !MenuDialog.GetMenuDialog().isActiveAndEnabled)
            ShowPause();
        else if (Input.GetKeyDown(KeyCode.Escape) && popup.activeSelf)
            Cancel();
    }

    public void ShowAnswerCorrect()
    {
        popup.SetActive(true);
        layout = panel.transform.Find("answerCorrect").gameObject;
        layout.SetActive(true);

        table.SetActive(false);
        toggle.isOn = false;

        option1 = layout.transform.Find("continue").gameObject.GetComponent<Button>();
        option2 = layout.transform.Find("exit").gameObject.GetComponent<Button>();

        option1.onClick.RemoveAllListeners();
        option2.onClick.RemoveAllListeners();

        option1.onClick.AddListener(RunFloorCleared);
        option2.onClick.AddListener(ReturnTitle);
    }

    public void ShowGameClear()
    {
        popup.SetActive(true);
        layout = panel.transform.Find("gameClear").gameObject;
        layout.SetActive(true);

        table.SetActive(false);
        toggle.isOn = false;

        option1 = layout.transform.Find("retry").gameObject.GetComponent<Button>();
        option2 = layout.transform.Find("exit").gameObject.GetComponent<Button>();

        option1.onClick.RemoveAllListeners();
        option2.onClick.RemoveAllListeners();

        option1.onClick.AddListener(Retry);
        option2.onClick.AddListener(ReturnTitle);
    }

    public void ShowGameOver()
    {
        popup.SetActive(true);
        layout = panel.transform.Find("gameOver").gameObject;
        layout.SetActive(true);

        GameObject solution = layout.transform.Find("solution").gameObject;
        GameObject solutionPanel = GameObject.Instantiate(answer.transform.Find("panel").gameObject, solution.transform);

        RectTransform solutionPanelRect = solutionPanel.GetComponent<RectTransform>();
        RectTransform solutionRect = solution.GetComponent<RectTransform>();

        solutionPanelRect.anchorMin = solutionRect.anchorMin;
        solutionPanelRect.anchorMax = solutionRect.anchorMax;
        solutionPanelRect.pivot = solutionRect.pivot;

        solutionPanelRect.anchoredPosition = solutionRect.anchoredPosition;

        float scaleX = solutionRect.rect.size.x / solutionPanelRect.rect.size.x;
        float scaleY = solutionRect.rect.size.y / solutionPanelRect.rect.size.y;

        solutionPanel.transform.localScale = new Vector3(scaleX, scaleY, 1f);

        table.SetActive(false);
        toggle.isOn = false;

        option1 = layout.transform.Find("retry").gameObject.GetComponent<Button>();
        option2 = layout.transform.Find("exit").gameObject.GetComponent<Button>();

        option1.onClick.RemoveAllListeners();
        option2.onClick.RemoveAllListeners();

        option1.onClick.AddListener(Retry);
        option2.onClick.AddListener(ReturnTitle);
    }

    public void ShowPause()
    {
        popup.SetActive(true);
        layout = panel.transform.Find("pause").gameObject;
        layout.SetActive(true);

        table.SetActive(false);
        toggle.isOn = false;

        option1 = layout.transform.Find("exit").gameObject.GetComponent<Button>();
        option2 = layout.transform.Find("retry").gameObject.GetComponent<Button>();
        option3 = layout.transform.Find("cancel").gameObject.GetComponent<Button>();

        option1.onClick.RemoveAllListeners();
        option2.onClick.RemoveAllListeners();
        option3.onClick.RemoveAllListeners();

        option1.onClick.AddListener(ReturnTitle);
        option2.onClick.AddListener(Retry);
        option3.onClick.AddListener(Cancel);
    }

    private void ReturnTitle()
    {
        if (!panel.transform.Find("pause").gameObject.activeSelf)
        {
            if (panel.transform.Find("gameClear").gameObject.activeSelf)
                GameManager.Instance.StartNewRun();

            GameManager.Instance.AdvanceFloor();
        }
        SceneManager.LoadScene("title");
    }

    private void Retry()
    {
        GameManager.Instance.StartNewRun();
        RunFloorCleared();
    }

    private void Cancel()
    {
        layout.SetActive(false);
        popup.SetActive(false);
    }

    private void RunFloorCleared()
    {
        floorManager.StartCoroutine(floorManager.ResolveFloorClear());
    }
}
