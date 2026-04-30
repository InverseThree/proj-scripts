using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator
{
    private string[] npcLabels = FloorManager.npcLabels;

    private int npcCount;
    
    // Track which statement types have been used
    HashSet<StatementList> usedStatementTypes = new HashSet<StatementList>();

    public PuzzleData Generate(int floorIndex, System.Random rng)
    {
        npcCount = floorIndex <= 3 ? 2 : floorIndex <= 6 ? 3 : floorIndex <= 9 ? 4 : 5;
        bool peasantRequired = floorIndex >= 10;

        var puzzle = new PuzzleData
        {
            floorIndex = floorIndex,
            npcCount = npcCount,
            peasantRequired = peasantRequired,
            npcInfo = new PuzzleData.NPCInfo[npcCount],
            role = new Role[npcCount],
            playerGuesses = new int[npcCount]
        };

        for (int i = 0; i < npcCount; i++)
            puzzle.playerGuesses[i] = 0;

        for (int attempt = 0; attempt < 1000; attempt++)
        {
            usedStatementTypes.Clear();

            do
            {
                GenerateSolutions(puzzle, rng);
                GenerateStatements(puzzle, floorIndex, rng);
            } while (!PuzzleValidator.CheckValid(puzzle, puzzle.role));

            var solutions = PuzzleValidator.FindSolutions(puzzle);
            Debug.Log($"Attempt {attempt}: solutions = {solutions.Count}");

            if (solutions.Count == 1)  // Exactly one solution
                break;
        }

        for (int i = 0; i < npcCount; i++)
        {
            puzzle.npcInfo[i].label = npcLabels[i];
            puzzle.npcInfo[i].discovered = false;
            puzzle.npcInfo[i].statementText = puzzle.npcInfo[i].statement.ToText(npcLabels, i);
        }
        return puzzle;
    }

    private void GenerateSolutions(PuzzleData puzzle, System.Random rng)
    {
        for (int i = 0; i < puzzle.npcCount; i++)
            puzzle.role[i] = (Role)rng.Next(1, 3);

        if (puzzle.peasantRequired && !Array.Exists(puzzle.role, r => r == Role.Peasant))
            puzzle.role[rng.Next(puzzle.npcCount)] = Role.Peasant;
    }

    private void GenerateStatements(PuzzleData puzzle, int floor, System.Random rng)
    {
        var probs = PuzzleData.DifficultyProfile.GetTierProbabilities(floor);
        float easy = probs.easy, medium = probs.medium, hard = probs.hard;
        bool isPeasantFloor = puzzle.peasantRequired;

        for (int npc = 0; npc < puzzle.npcCount; npc++)
        {
            float roll = (float)rng.NextDouble();
            StatementDifficulty tier = roll < easy ? StatementDifficulty.Easy : roll < easy + medium ? StatementDifficulty.Medium : StatementDifficulty.Hard;

            var stmt = new StatementParser();

            stmt.statement = StatementsPool(tier, isPeasantFloor, rng);
            stmt.variant = rng.Next(stmt.GetVariantCount(npcCount));

            AssignTargets(stmt, npc, puzzle.npcCount, rng);

            puzzle.npcInfo[npc] = new PuzzleData.NPCInfo { statement = stmt };

            usedStatementTypes.Add(stmt.statement);
        }
    }

    private StatementList StatementsPool(StatementDifficulty tier, bool isPeasantFloor, System.Random rng)
    {
        var pool = new List<StatementList>();

        if (tier == StatementDifficulty.Easy)
        {
            pool.Add(StatementList.IsKnight);
            pool.Add(StatementList.IsKnave);
        }
        else if (tier == StatementDifficulty.Medium)
        {
            pool.Add(StatementList.IsKnight); // variants 0/2 are medium
            pool.Add(StatementList.BothAreKnights);
            pool.Add(StatementList.BothAreKnaves);
            pool.Add(StatementList.RoleSame);
        }
        else // Hard
        {
            pool.AddRange(new[] { StatementList.EitherKnightOrKnight, StatementList.EitherKnightOrKnave,
                    StatementList.EitherKnaveOrKnave, StatementList.ExactlyOneIsKnight,
                    StatementList.ExactlyOneIsKnave });
            if (!isPeasantFloor)
                pool.AddRange(new[] { StatementList.OnlyKnightSayKnight, StatementList.OnlyKnightSayKnave,
                        StatementList.OnlyKnaveSayKnight, StatementList.OnlyKnaveSayKnave });
        }

        pool.RemoveAll(s => usedStatementTypes.Contains(s));

        if (pool.Count == 0)
            pool.Add(StatementList.IsKnight);

        return pool[rng.Next(pool.Count)];
    }

    private void AssignTargets(StatementParser stmt, int npc, int npcCount, System.Random rng)
    {
        if (stmt.statement == StatementList.IsKnight || stmt.statement == StatementList.IsKnave || stmt.statement.ToString().Contains("Only"))
            stmt.a = PickDistinctTarget(npc, npcCount, rng);
        else if (stmt.statement.ToString().Contains("Either") || stmt.statement.ToString().Contains("Both") || stmt.statement == StatementList.RoleSame)
        {
            stmt.a = PickDistinctTarget(npc, npcCount, rng);
            stmt.b = PickDistinctTarget(npc, npcCount, rng, stmt.a);  // Already excludes both 'npc' and 'stmt.a'

            if (stmt.a == npc || stmt.b == npc)
            {
                stmt.a = PickDistinctTarget(npc, npcCount, rng); 
                stmt.b = PickDistinctTarget(npc, npcCount, rng);
            }

            // If 'b' ended up same as 'a', try again with different method
            if (stmt.b == stmt.a || stmt.b == -1)
            {
                // For 2-NPC floors, use speaker + other NPC
                stmt.a = npc;  // Speaker
                stmt.b = npc == 0 ? 1 : 0;  // Other NPC
            }
        }
    }

    private int PickDistinctTarget(int forbidden, int npcCount, System.Random rng, params int[] extra)
    {
        List<int> candidates = new List<int>();
        for (int i = 0; i < npcCount; i++)
        {
            if (i == forbidden) continue;
            bool blocked = false;
            foreach (int f in extra) if (i == f) blocked = true;
            if (!blocked) candidates.Add(i);
        }
        return candidates.Count > 0 ? candidates[rng.Next(candidates.Count)] : 0;
    }
}
