using UnityEngine;
using Fungus;

public class StatementController : MonoBehaviour
{
    public Flowchart flowchart;
    public SayDialog say;

    public string runtimeBlock = "npcStatement";

    public string lineVariable = "NPCLine";

    public void ShowLine(string line) {
        flowchart.SetStringVariable(lineVariable, line);
    }

    public void ShowName(string name) {
        say.SetCharacterName(name, Color.gray);
    }
}
