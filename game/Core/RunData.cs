using System;
using UnityEngine;

[Serializable]
public class RunSaveData
{
    public int currentFloor;
    public int currentHealth;
    public ItemType heldItem;
    public RunModifierState modifierState;
    public PuzzleData currentPuzzle;
}

