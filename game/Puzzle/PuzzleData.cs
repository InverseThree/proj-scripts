using System;
using UnityEngine;

[Serializable]
public class PuzzleData
{
    public int floorIndex;
    public int npcCount;

    public bool peasantRequired;

    public NPCInfo[] npcInfo;
    public Role[] role;

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
        public string label;
        public StatementParser statement;
        public string statementText;
        public bool discovered;
        
        public int headMaterialIndex = -1;
        public int bodyMaterialIndex = -1;
    }

    public static class DifficultyProfile
    {
        public static (float easy, float medium, float hard) GetTierProbabilities(int floor)
        {
            if (floor <= 3) return (0.70f, 0.20f, 0.10f);
            if (floor <= 6) return (0.30f, 0.50f, 0.20f);
            if (floor <= 9) return (0.20f, 0.40f, 0.40f);
            return (0.10f, 0.35f, 0.55f);
        }
    }
}
