using UnityEngine;
using Fungus;

public class TriggerFlowchart : MonoBehaviour {

	public Flowchart flowchart;
	public string triggerInBlockName;
	public string triggerTag = "Player";

    private NPCController nearbyNPC;

    // protected FixedJoybutton joybutton;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag(triggerTag))
        {
            nearbyNPC = GetComponent<NPCController>();
        }
    }

    private void OnTriggerStay (Collider col)
    {
        if (col.CompareTag(triggerTag))
        {
            if ((Input.GetKeyDown("space")) && !SayDialog.GetSayDialog().isActiveAndEnabled && !MenuDialog.GetMenuDialog().isActiveAndEnabled)
            {
                var fc = GetFlowchart.Instance;

                nearbyNPC?.InteractShowLine();

                fc.ExecuteIfHasBlock(triggerInBlockName);

                nearbyNPC?.InteractShowName();
                nearbyNPC?.OnDialogueFinished();
            }
        }
    }
}
