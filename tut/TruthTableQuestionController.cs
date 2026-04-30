using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fungus;

public class TruthTableQuestionController : MonoBehaviour
{
    public TMP_Text[] rowNotA;
    public TMP_Text[] rowNotAandB;
    public TMP_Text[] rowLeft;
    public TMP_Text[] rowAorB;
    public TMP_Text[] rowImplies;
    public TMP_Text[] rowFinal;

    public bool[] A;
    public bool[] B;
    public bool[] C;

    public Flowchart flowchart;

    public Button submitButton;
    private Button[] truthbuttons;

    private List<TMP_Text[]> tmpTexts = new List<TMP_Text[]>();

    private bool result;

    private string targetBolck;

    private void Start()
    {
        tmpTexts.Add(rowNotA);
        tmpTexts.Add(rowNotAandB);
        tmpTexts.Add(rowLeft);
        tmpTexts.Add(rowAorB);
        tmpTexts.Add(rowImplies);
        tmpTexts.Add(rowFinal);
        truthbuttons = new Button[tmpTexts.Count * tmpTexts[0].Length];

        int index = 0;

        foreach (TMP_Text[] texts in tmpTexts)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                truthbuttons[index] = texts[i].transform.parent.gameObject.GetComponent<Button>();
                truthbuttons[index].onClick.AddListener(delegate{ SetSubmitButton(); });

                index++;
            }
        }
    }

    public void Check()
    {
        for (int i = 0; i < A.Length; i++)
        {
            bool notA = !A[i];
            bool notAandB = notA && B[i];
            bool left = !notAandB;

            bool AorB = A[i] || B[i];
            bool implies = !C[i] || AorB;

            bool final = left == implies;

            if (Parse(rowNotA[i]) != notA)
                result = false;
            else if (Parse(rowNotAandB[i]) != notAandB)
                result = false;
            else if (Parse(rowLeft[i]) != left)
                result = false;
            else if (Parse(rowAorB[i]) != AorB)
                result = false;
            else if (Parse(rowImplies[i]) != implies)
                result = false;
            else if (Parse(rowFinal[i]) != final)
                result = false;
            else
                result = true;
        }

        HandleResult(result);
    }

    private void SetSubmitButton()
    {
        for (int i = 0; i < rowNotA.Length; i++)
        {
            if (rowNotA[i].text == "" || rowNotAandB[i].text == "" || rowLeft[i].text == "" || rowAorB[i].text == "" || rowImplies[i].text == "" || rowFinal[i].text == "")
            {
                submitButton.interactable = false;
                return;
            }
        }

        submitButton.interactable = true;
    }

    private bool Parse(TMP_Text t)
    {
        return t.text.Trim().ToUpper() == "T";
    }

    private void HandleResult(bool result)
    {
        flowchart.SetBooleanVariable("ansCorrect", result);

        targetBolck = result ? "correct" : "wrong";
        flowchart.ExecuteBlock(targetBolck);
    }
}
