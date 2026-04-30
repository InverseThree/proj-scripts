using UnityEngine;

/// <summary>
/// One spawned NPC in the room.
/// Holds its label and generated statement text.
/// </summary>
public class NPCController: MonoBehaviour
{
    [Header("Optional Visual References")]
    public NPCAppearance appearance;

    private FloorManager floorManager;
    private int npcIndex;
    private string npcName;
    private string statementText;
    private StatementController statementController;

    public string Label => npcName;

    public void Setup(FloorManager owner, int index, PuzzleData puzzle, StatementController controller)
    {
        floorManager = owner;
        npcIndex = index;
        npcName = puzzle.npcInfo[index].label;
        statementText = puzzle.npcInfo[index].statementText;
        statementController = controller;

        gameObject.name = $"NPC_{npcName}";

        Debug.Log($"NPC {index} Setup - name: {npcName}, statement: {statementText}");
        Debug.Log($"NPC {index} statementController: " + (controller == null ? "NULL" : "OK"));
    }

    /// <summary>
    /// Call this from your player interaction system when player presses Z near the NPC.
    /// </summary>
    public void InteractShowLine()
    {
        statementController.ShowLine(statementText);
    }

    public void InteractShowName()
    {
        statementController.ShowName(npcName);
    }

    /// <summary>
    /// Called by FungusDialogueRunner after the line closes.
    /// This is where we mark the statement as discovered.
    /// </summary>
    public void OnDialogueFinished()
    {
        floorManager?.RevealStatement(npcIndex);
    }
}
