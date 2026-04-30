using UnityEngine;

public class NPCAppearance : MonoBehaviour
{
    public Renderer headRenderer;
    public Renderer bodyRenderer;

    public void Apply(Material headMat, Material bodyMat)
    {
        if (headRenderer != null)
            headRenderer.sharedMaterial = headMat;

        if (bodyRenderer != null)
            bodyRenderer.sharedMaterial = bodyMat;
    }
}
