using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class HintController : MonoBehaviour
{
    private GameObject hint;

    private void Awake()
    {
        hint = GameObject.FindGameObjectWithTag("hint");
        SetHint();
    }

    private void Update()
    {
        if (MenuDialog.GetMenuDialog().isActiveAndEnabled && hint.activeSelf)
            SetHint();

        if (Input.GetKeyDown(KeyCode.H) && !hint.activeSelf && !MenuDialog.GetMenuDialog().isActiveAndEnabled)
            SetHint();
        else if (Input.GetKeyDown(KeyCode.H) && hint.activeSelf)
            SetHint();
    }

    private void SetHint()
    {
        hint.SetActive(!hint.activeSelf);
    }
}
