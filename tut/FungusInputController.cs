using UnityEngine;
using Fungus;

public class FungusInputController : MonoBehaviour
{
    private MonoBehaviour input;

    public void DisableInput()
    {
        SayDialog sayDialog = FindObjectOfType<SayDialog>();

        if (sayDialog == null) return;

        // Find the component that handles input
        MonoBehaviour[] monobehaviours = sayDialog.GetComponents<MonoBehaviour>();

        foreach (var behaviour in monobehaviours)
        {
            // Look for input-related behaviour
            if (behaviour.GetType().Name.ToLower().Contains("input"))
            {
                input= behaviour;
                behaviour.enabled = false;
            }
        }
    }

    public void EnableInput()
    {
        if (input != null)
            input.enabled = true;
    }
}
