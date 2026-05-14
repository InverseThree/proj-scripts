using UnityEngine;

public class NPCController: MonoBehaviour
{
    [Header("Optional Visual References")]
    public NPCAppearance appearance;

    private FloorManager floorManager;
    private int npcIndex;
    public int npcSpawnedIndex;
    public string npcName;
    public string statementText;
    private StatementController statementController;

    public string Label => npcName;

    public void Setup(FloorManager owner, int index, int spawnedIndex, PuzzleData puzzle, StatementController controller)
    {
        floorManager = owner;
        npcIndex = index;
        npcSpawnedIndex = spawnedIndex;
        npcName = puzzle.npcInfo[index].label;
        statementText = puzzle.npcInfo[index].statementText;
        statementController = controller;

        gameObject.name = $"NPC_{npcName}";

        Debug.Log($"NPC {index} Setup - name: {npcName}, statement: {statementText}");
        Debug.Log($"NPC {index} statementController: " + (controller == null ? "NULL" : "OK"));
    }

    public void InteractShowLine()
    {
        statementController.ShowLine(statementText);
    }

    public void InteractShowName()
    {
        statementController.ShowName(npcName);
    }

    public void OnDialogueFinished()
    {
        floorManager?.RevealStatement(npcIndex);
    }
}
