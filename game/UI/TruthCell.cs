using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TriValue
{
    Empty,
    True,
    False
}

public class TruthCell : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Image background;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color correctColor = new Color(0.7f, 1f, 0.7f);
    [SerializeField] private Color wrongColor = new Color(1f, 0.7f, 0.7f);
    [SerializeField] private Color incompleteColor = new Color(1f, 1f, 0.6f);

    public TriValue Value { get; private set; } = TriValue.Empty;

    public void OnClick()
    {
        Value = Value switch
        {
            TriValue.Empty => TriValue.True,
            TriValue.True => TriValue.False,
            _ => TriValue.Empty
        };

        Refresh();
    }

    public void SetState(TriValue newValue)
    {
        Value = newValue;
        Refresh();
    }

    public void ResetVisual()
    {
        if (background != null)
            background.color = normalColor;
    }

    public void MarkCorrect(bool isCorrect)
    {
        if (background != null)
            background.color = isCorrect ? correctColor : wrongColor;
    }

    public void MarkIncomplete()
    {
        if (background != null)
            background.color = incompleteColor;
    }

    public bool? AsNullableBool()
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
