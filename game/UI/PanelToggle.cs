using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class PanelToggle : MonoBehaviour
{
    [Header("Setup")]
    public RectTransform sideMenuContainer;
    public Toggle expander;

    [Header("Size Controls")]
    public int collapsedWidth = 0;
    public int expandedWidth = 1200;

    [Header("Animation Controls")]
    public bool instantChange;
    public AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float animationDuration = 0.3f;

    [Header("Testing")]
    public bool testingActive;
    public bool setToggleTo;

    private Coroutine animationRoutine;

    private void Reset()
    {
        sideMenuContainer = GetComponent<RectTransform>();
        expander = GetComponentInChildren<Toggle>();
    }

    private void Awake()
    {
        testingActive = false;
        expander.onValueChanged.AddListener(OnExpanderValueChanged);
    }

    private void FixedUpdate()
    {
        if (SayDialog.GetSayDialog().isActiveAndEnabled || MenuDialog.GetMenuDialog().isActiveAndEnabled)
            expander.interactable = false;
        else
            expander.interactable = true;
    }

    private void OnDestory()
    {
        expander.onValueChanged.RemoveListener(OnExpanderValueChanged);
    }

    private void OnValidate()
    {
        if (!testingActive)
            return;

        float destinationWidth = setToggleTo ? expandedWidth : collapsedWidth;
        sideMenuContainer.sizeDelta = new Vector2(destinationWidth, sideMenuContainer.sizeDelta.y);
    }

    private void OnExpanderValueChanged(bool value){
        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        StartCoroutine(AnimateChange(value));
    }

    private IEnumerator AnimateChange(bool value)
    {
        float destinationWidth = value ? expandedWidth : collapsedWidth;

        if (instantChange)
        {
            sideMenuContainer.sizeDelta = new Vector2(destinationWidth, sideMenuContainer.sizeDelta.y);
            yield break;
        }

        float startWidth = sideMenuContainer.sizeDelta.x;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);
            float easedT = easingCurve.Evaluate(t);
            float currentWidth = Mathf.Lerp(startWidth, destinationWidth, easedT);
            sideMenuContainer.sizeDelta = new Vector2(currentWidth, sideMenuContainer.sizeDelta.y);
            yield return null;
        }

        sideMenuContainer.sizeDelta = new Vector2(destinationWidth, sideMenuContainer.sizeDelta.y);
    }
}
