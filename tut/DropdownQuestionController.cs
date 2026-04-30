using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fungus;

public class DropdownQuestionController : MonoBehaviour
{
    public TMP_Dropdown[] dropdowns;
    public int[] correctAnswers;

    public Flowchart flowchart;
    public Button submitButton;

    private string targetBolck;

    private void Start()
    {
        for (int i = 0; i < dropdowns.Length; i++)
            dropdowns[i].onValueChanged.AddListener(delegate{ SetSubmitButton(); });
    }

    private void SetSubmitButton()
    {
        for (int i = 0; i < dropdowns.Length; i++)
        {
            if (dropdowns[i].value == 0)
            {
                submitButton.interactable = false;
                return;
            }
        }

        submitButton.interactable = true;
    }

    public void CheckAnswers()
    {
        bool allCorrect = true;

        for (int i = 0; i < dropdowns.Length; i++)
        {
            if (dropdowns[i].value != correctAnswers[i])
            {
                allCorrect = false;
                break;
            }
        }

        HandleResult(allCorrect);
    }

    private void HandleResult(bool result)
    {
        flowchart.SetBooleanVariable("ansCorrect", result);

        targetBolck = result ? "correct" : "wrong";
        flowchart.ExecuteBlock(targetBolck);
    }
}
