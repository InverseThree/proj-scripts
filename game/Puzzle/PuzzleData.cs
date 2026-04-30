using System;
using UnityEngine;

[Serializable]
public class PuzzleData
{
    public int floorIndex;
    public int npcCount;

    // Floor rules, not necessarily public knowledge.
    // If peasant is possible on this floor, do NOT show truth-table draft.
    public bool peasantRequired;

    public NPCInfo[] npcInfo;
    public Role[] role;

    // Player guesses saved as ints.
    // 0 = no answer yet
    //  1 = Knight
    //  2 = Knave
    //  3 = Peasant
    public int[] playerGuesses;

    public bool HasPeasantInSolution()
    {
        if (role == null)
            return false;

        for (int i = 0; i < role.Length; i++)
        {
            if (role[i] == Role.Peasant)
                return true;
        }

        return false;
    }

    public bool PeasantMayAppearOnThisFloor()
    {
        return peasantRequired;
    }

    public class NPCInfo
    {
        public string label;                  // P, Q, R, S, T
        public StatementParser statement;     // The actual statement object
        public string statementText;          // Cached text for UI/dialog
        public bool discovered;               // Has player talked to this NPC yet?

        // Saved visual appearance so load/continue shows same look
        public int headMaterialIndex = -1;
        public int bodyMaterialIndex = -1;

        // If an item revealed this identity, we can restore that on load
        public bool identityRevealedByItem = false;
    }

    public static class DifficultyProfile
    {
        public static (float easy, float medium, float hard) GetTierProbabilities(int floor)
        {
            if (floor <= 3) return (0.70f, 0.20f, 0.10f);
            if (floor <= 6) return (0.30f, 0.50f, 0.20f);
            if (floor <= 9) return (0.20f, 0.40f, 0.40f);
            return (0.10f, 0.35f, 0.55f); // Floor 10
        }
    }
}
