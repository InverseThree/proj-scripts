using System;
using System.Collections.Generic;

[Serializable]
public class PuzzleData
{
    public int floorIndex;
    public int npcCount;
    public bool peasantRequired;

    public NPCInfo[] npcInfo;
    public Role[] role;

    // 0 = unset, 1 = Knight, 2 = Knave, 3 = Peasant
    public int[] playerGuesses;

    public FloorSpecialState floorState = new FloorSpecialState();
    public List<string> hints = new List<string>();

    public bool HasPeasantInSolution()
    {
        if (role == null)
            return false;

        for (int i = 0; i < role.Length; i++)
            if (role[i] == Role.Peasant)
                return true;

        return false;
    }

    public bool PeasantEnabled()
    {
        return peasantRequired;
    }

    [Serializable]
    public class NPCInfo
    {
        public string label;
        public StatementParser statement;
        public string statementText;
        public bool discovered;

        public int headMaterialIndex = -1;
        public int bodyMaterialIndex = -1;

        public bool identityRevealed = false;
    }

    [Serializable]
    public class LampPairHint
    {
        public int a = -1;
        public int b = -1;
        public bool same = false;
    }

    [Serializable]
    public class FloorSpecialState
    {
        public bool shieldActive = false;
        public bool hourglassRewinded = false;
        public bool talismanPrevented = false;
        public bool shardResolved = false;
        public bool scytheUsed = false;

        public int mirrorHintA = -1;
        public int mirrorHintB = -1;

        public int lampHiddenIndex = -1;

        public int lampWishesSpent = 0;
        public int lampIdentitySpent = 0;
        public int lampPairSpent = 0;
        public int lampCountSpent = 0;

        public int lampIdentityIndex = -1;
        public List<LampPairHint> lampPairHints = new List<LampPairHint>();
        public bool lampCountGranted = false;
        public int lampKnightCount = -1;
        public int lampKnaveCount = -1;

        public bool scrollViewed = false;
        public int scrollRevealNumber = -1;
        public bool scrollRevealRole = false;

        public void ClearDerivedFloorHints()
        {
            mirrorHintA = -1;
            mirrorHintB = -1;
            lampIdentityIndex = -1;
            lampPairHints.Clear();
            lampCountGranted = false;
            lampKnightCount = -1;
            lampKnaveCount = -1;
            scrollViewed = false;
            scrollRevealNumber = -1;
        }
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
