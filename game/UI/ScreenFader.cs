using System.Collections;
using UnityEngine;

/// <summary>
/// Put this on a full-screen black UI Image with a CanvasGroup.
/// </summary>
public class ScreenFader : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    public IEnumerator FadeOut(float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / duration;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public IEnumerator FadeIn(float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - (t / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}
