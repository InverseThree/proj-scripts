using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hitboxFix : MonoBehaviour
{
    public Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        image.alphaHitTestMinimumThreshold = 0.1f;
    }
}
