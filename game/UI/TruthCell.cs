using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TruthCell : MonoBehaviour
{
    private enum TriValue
    {
        Empty,
        True,
        False
    }

    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Image background;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color correctColor = new Color(0.7f, 1f, 0.7f);
    [SerializeField] private Color wrongColor = new Color(1f, 0.7f, 0.7f);
    [SerializeField] private Color incompleteColor = new Color(1f, 1f, 0.6f);

    private TriValue Value { get; set; } = TriValue.Empty;

    private void OnClick()
    {
        Value = Value switch
        {
            TriValue.Empty => TriValue.True,
            TriValue.True => TriValue.False,
            _ => TriValue.Empty
        };

        Refresh();
    }

    private void SetState(TriValue newValue)
    {
        Value = newValue;
        Refresh();
    }

    private void ResetVisual()
    {
        if (background != null)
            background.color = normalColor;
    }

    private void MarkCorrect(bool isCorrect)
    {
        if (background != null)
            background.color = isCorrect ? correctColor : wrongColor;
    }

    private void MarkIncomplete()
    {
        if (background != null)
            background.color = incompleteColor;
    }

    private bool? AsNullableBool()
    {
        return Value switch
        {
            TriValue.True => true,
            TriValue.False => false,
            _ => null
        };
    }

    private void Refresh()
    {
        if (label == null) return;

        label.text = Value switch
        {
            TriValue.True => "T",
            TriValue.False => "F",
            _ => ""
        };

        ResetVisual();
    }
}
