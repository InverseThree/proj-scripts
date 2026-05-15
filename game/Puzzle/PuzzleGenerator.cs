using System;
using System.Collections.Generic;
using UnityEngine;

public struct StatementKey : System.IEquatable<StatementKey>
{
    public StatementList type;
    public int variant;

    public StatementKey(StatementList type, int variant)
    {
        this.type = type;
        this.variant = variant;
    }

    public bool Equals(StatementKey other)
    {
        return type == other.type && variant == other.variant;
    }

    public override int GetHashCode()
    {
        return ((int)type * 397) ^ variant;
    }
}

public class PuzzleGenerator
{
    private string[] npcLabels = FloorManager.npcLabels;

    private int npcCount;

    private PuzzleData lastPuzzle;

    private List<StatementList> statementPool = new List<StatementList>();

    HashSet<StatementKey> usedStatementVariants = new();

    public PuzzleData Generate(int floorIndex, System.Random rng, bool scytheUsed, List<int> selectedIndices)
    {
        RunModifierState mods = GameManager.Instance.modifierState;

        foreach (StatementList statement in Enum.GetValues(typeof(StatementList)))
            statementPool.Add(statement);

        if (mods.brushActive)
            npcCount = rng.Next(2, 7);
        else
            npcCount = floorIndex <= 3 ? 2 : floorIndex <= 6 ? 3 : floorIndex <= 9 ? 4 : 5;

        if (mods.coinActive)
            npcCount += 1;

        bool peasantRequired = floorIndex >= 10;

        PuzzleData puzzle = new PuzzleData
        {
            floorIndex = floorIndex,
            npcCount = npcCount,
            peasantRequired = peasantRequired,
            npcInfo = new PuzzleData.NPCInfo[npcCount],
            role = new Role[npcCount],
            playerGuesses = new int[npcCount],
            floorState = new PuzzleData.FloorSpecialState(),
            hints = new List<string>()
        };

        for (int i = 0; i < npcCount; i++)
            puzzle.playerGuesses[i] = 0;

        for (int attempt = 0; attempt < 10000; attempt++)
        {
            do
            {
                GenerateSolutions(puzzle, rng, scytheUsed, selectedIndices);
                GenerateStatements(puzzle, floorIndex, rng);

                for (int i = 0; i < puzzle.role.Length; i++)
                {
                    if (puzzle.role[i] == Role.None)
                    {
                        puzzle.role[i] = lastPuzzle.role[i];
                        puzzle.npcInfo[i] = lastPuzzle.npcInfo[i];
                    }
                }

                if (puzzle.peasantRequired && !Array.Exists(puzzle.role, r => r == Role.Peasant))
                    puzzle.role[rng.Next(puzzle.npcCount)] = Role.Peasant;
            } while (!PuzzleValidator.CheckValid(puzzle, puzzle.role));

            List<Role[]> solutions = PuzzleValidator.FindSolutions(puzzle);
            Debug.Log($"Attempt {attempt}: solutions = {solutions.Count}");

            if (solutions.Count == 1)
            {
                for (int i = 0; i < npcCount; i++)
                {
                    if (!scytheUsed || selectedIndices.Contains(i))
                    {
                        puzzle.npcInfo[i].label = npcLabels[i]; 
                        puzzle.npcInfo[i].statementText = puzzle.npcInfo[i].statement.ToText(npcLabels, i);
                        puzzle.npcInfo[i].discovered = false;
                        puzzle.npcInfo[i].identityRevealed = false;
                        puzzle.npcInfo[i].headMaterialIndex = -1;
                        puzzle.npcInfo[i].bodyMaterialIndex = -1;
                    }
                }

                break;
            }
        }

        lastPuzzle = puzzle;
        return puzzle;
    }

    public bool RerollSelected(PuzzleData puzzle, FloorManager floorManager, List<int> selectedIndices, System.Random rng)
    {
        if (selectedIndices == null || selectedIndices.Count == 0)
            return false;

        List<int> indicesList = new List<int>();
        foreach (int index in selectedIndices)
        {
            for (int i = 0; i < puzzle.npcCount; i++)
            {
                if (floorManager.spawnedNPCs[i].npcSpawnedIndex == index)
                {
                    indicesList.Add(floorManager.spawnedNPCs[i].npcIndex);
                    break;
                }
            }
        }

        GameManager.Instance.currentPuzzle = Generate(puzzle.floorIndex, rng, true, indicesList);

        return true;
    }

    private void GenerateSolutions(PuzzleData puzzle, System.Random rng, bool scytheUsed, List<int> selectedIndices)
    {
        for (int i = 0; i < puzzle.npcCount; i++)
        {
            if ((scytheUsed && selectedIndices.Contains(i)) || !scytheUsed)
                puzzle.role[i] = (Role)rng.Next(1, 3);
            else
                puzzle.role[i] = (Role)0;
        }
    }

    private void GenerateStatements(PuzzleData puzzle, int floor, System.Random rng)
    {
        usedStatementVariants.Clear();

        var probs = PuzzleData.DifficultyProfile.GetTierProbabilities(floor);

        for (int npc = 0; npc < puzzle.npcCount; npc++)
        {
            StatementList type;
            int variant;

            int tries = 0;

            float roll = (float)rng.NextDouble();

            StatementDifficulty tier = roll < probs.easy ? StatementDifficulty.Easy : roll < probs.easy + probs.medium ? StatementDifficulty.Medium : StatementDifficulty.Hard;

            StatementParser stmt = new StatementParser();

            do
            {
                StatementParser temp = new StatementParser();

                do{
                    type = GetRandomStatement(rng);
                    temp.statement = type;

                    variant = rng.Next(temp.GetVariantCount(npcCount));
                    if (variant < 1)
                        continue;

                    temp.variant = variant;
                } while (temp.GetSuggestedDifficulty(npcCount) != tier || (puzzle.peasantRequired && !temp.IsCompatibleWithPeasantFloor()));

                tries++;
                if (tries > 10000)
                    break;
            } while (usedStatementVariants.Contains(new StatementKey(type, variant)));

            stmt.statement = type;
            stmt.variant = variant;

            usedStatementVariants.Add(new StatementKey(type, variant));

            AssignTargets(stmt, npc, puzzle.npcCount, rng);

            puzzle.npcInfo[npc] = new PuzzleData.NPCInfo
            {
                statement = stmt
            };
        }
    }

    private StatementList GetRandomStatement(System.Random rng)
    {
        return statementPool[rng.Next(statementPool.Count)];
    }

    private void AssignTargets(StatementParser stmt, int selfIndex, int npcCount, System.Random rng)
    {
        stmt.a = PickDistinctTarget(selfIndex, npcCount, rng);
        stmt.b = PickDistinctTarget(selfIndex, npcCount, rng, stmt.a);
        stmt.c = PickDistinctTarget(selfIndex, npcCount, rng, stmt.a, stmt.b);
        stmt.d = PickDistinctTarget(selfIndex, npcCount, rng, stmt.a, stmt.b, stmt.c);
        stmt.e = PickDistinctTarget(selfIndex, npcCount, rng, stmt.a, stmt.b, stmt.c, stmt.d);
    }

    private int PickDistinctTarget(int forbidden, int npcCount, System.Random rng, params int[] extra)
    {
        List<int> candidates = new List<int>();

        for (int i = 0; i < npcCount; i++)
        {
            if (i == forbidden) continue;

            bool blocked = false;
            for (int j = 0; j < extra.Length; j++)
            {
                if (i == extra[j])
                {
                    blocked = true;
                    break;
                }
            }

            if (!blocked)
                candidates.Add(i);
        }

        if (candidates.Count == 0)
            return Mathf.Clamp(forbidden == 0 ? 1 : 0, 0, npcCount - 1);

        return candidates[rng.Next(candidates.Count)];
    }
}
