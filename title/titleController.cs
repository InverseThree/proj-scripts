using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class titleController : MonoBehaviour
{
    private Button exitButton;
    private Button settingsButton;
    private Button playButton;

    private Button creditButton;
    private Button returnButton;

    private Animator titleAnim;

    private GameObject playMenu;
    private GameObject credit;
    private GameObject option;

    private static int lastState;
    private static int titlePlay = Animator.StringToHash("titlePlay");
    private static int titleOption = Animator.StringToHash("titleOption");
    private static int titlePlayOption= Animator.StringToHash("titlePlayOption");
    private static int titlePlayOptionInverted = Animator.StringToHash("titlePlayOptionInverted");

    private void Start()
    {
       exitButton = GameObject.FindGameObjectWithTag("exitButton").GetComponent<Button>();
       settingsButton = GameObject.FindGameObjectWithTag("settingsButton").GetComponent<Button>();
       playButton = GameObject.FindGameObjectWithTag("playButton").GetComponent<Button>();

       creditButton = GameObject.FindGameObjectWithTag("creditButton").GetComponent<Button>();
       returnButton = GameObject.FindGameObjectWithTag("returnButton").GetComponent<Button>();

       titleAnim = GetComponent<Animator>();

       playMenu = GameObject.FindGameObjectWithTag("playMenu");
       credit = GameObject.FindGameObjectWithTag("credit");
       option = GameObject.FindGameObjectWithTag("option");

       creditButton.onClick.AddListener(ShowCredit);
       returnButton.onClick.AddListener(ExitCredit);

       playMenu.SetActive(false);
       credit.SetActive(false);
       option.SetActive(false);

       exitButton.onClick.AddListener(Exit);
       settingsButton.onClick.AddListener(Option);
       playButton.onClick.AddListener(Play);
    }

    private void ShowCredit()
    {
        credit.SetActive(true);
    }

    private void ExitCredit()
    {
        credit.SetActive(false);
    }

    private void Exit()
    {
        Application.Quit();
    }

    private void Play()
    {
        lastState = titleAnim.GetCurrentAnimatorStateInfo(0).shortNameHash;

        if (lastState == titlePlay || lastState == titlePlayOptionInverted)
            titleAnim.Play("titlePlayInverted");
        else if (lastState == titleOption || lastState == titlePlayOption)
            titleAnim.Play("titlePlayOptionInverted");
        else
            titleAnim.Play("titlePlay");

        StartCoroutine("SetUI", false);
    }

    private void Option()
    {
        lastState = titleAnim.GetCurrentAnimatorStateInfo(0).shortNameHash;

        if (lastState == titleOption || lastState == titlePlayOption)
            titleAnim.Play("titleOptionInverted");
        else if (lastState == titlePlay || lastState == titlePlayOptionInverted)
            titleAnim.Play("titlePlayOption");
        else
            titleAnim.Play("titleOption");

        StartCoroutine("SetUI", true);
    }

    private IEnumerator SetUI(bool buttonOption)
    {
        if (!playMenu.activeSelf && !buttonOption)
        {
            option.SetActive(false);

            yield return new WaitForSeconds(0.25f);
            playMenu.SetActive(true);
        }
        else if (!option.activeSelf && buttonOption)
        {
            playMenu.SetActive(false);

            yield return new WaitForSeconds(0.25f);
            option.SetActive(true);
        }
        else if (playMenu.activeSelf || option.activeSelf)
        {
            playMenu.SetActive(false);
            option.SetActive(false);
        }
    }
}
