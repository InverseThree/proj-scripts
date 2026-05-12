using System.Collections.Generic;
using UnityEngine;

public static class PuzzleValidator
{
    public static List<Role[]> FindSolutions(PuzzleData puzzle)
    {
        List<Role[]> valid = new List<Role[]>();
        Role[] current = new Role[puzzle.npcCount];

        Recurse(0, 0);
        return valid;

        void Recurse(int index, int peasantCount)
        {
            if (index >= puzzle.npcCount)
            {
                if (puzzle.peasantRequired && peasantCount != 1) return;

                if (CheckValid(puzzle, current))
                {
                    Role[] copy = new Role[current.Length];
                    current.CopyTo(copy, 0);
                    valid.Add(copy);
                }
                return;
            }

            current[index] = Role.Knight;
            Recurse(index + 1, peasantCount);

            current[index] = Role.Knave;
            Recurse(index + 1, peasantCount);

            if (puzzle.peasantRequired && peasantCount == 0)
            {
                current[index] = Role.Peasant;
                Recurse(index + 1, peasantCount + 1);
            }
        }
    }

    public static bool CheckValid(PuzzleData puzzle, Role[] assignment)
    {
        for (int i = 0; i < puzzle.npcCount; i++)
        {
            bool statementTruth = puzzle.npcInfo[i].statement.Evaluate(assignment, i);
            Role role = assignment[i];

            switch (role)
            {
                case Role.Knight:
                    if (!statementTruth) return false;
                    break;

                case Role.Knave:
                    if (statementTruth) return false;
                    break;

                case Role.Peasant:
                    // unrestricted
                    break;
            }
        }

        return true;
    }
}
