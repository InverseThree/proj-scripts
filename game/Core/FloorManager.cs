using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FloorManager : MonoBehaviour
{
    [Header("Spawning")]
    public GameObject npcPrefab;
    public Transform[] npcSpawnPoints;
    public static string[] npcLabels = { "P", "Q", "R", "S", "T", "U" };

    [Header("Dialogue")]
    public StatementController statementController;

    [Header("Appearance")]
    public Material[] headMaterials; 
    public Material[] bodyMaterials;

    [Header("UI")]
    public AnswerPanelController answerPanelController;
    public PopupPanelController popupPanelController;

    [Header("Flow")]
    public ScreenFader screenFader;
    public int finalFloor = 10;

    private readonly List<NPCController> spawnedNpcs = new List<NPCController>();
    private System.Random rng = new System.Random();

    private PuzzleGenerator generator = new PuzzleGenerator();

    private PuzzleData currentPuzzle => GameManager.Instance.currentPuzzle;

    List<int> usedSpawnIndices = new List<int>();

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();

        statementController = FindObjectOfType<StatementController>();
        answerPanelController = FindObjectOfType<AnswerPanelController>();
        screenFader = FindObjectOfType<ScreenFader>();
        popupPanelController = FindObjectOfType<PopupPanelController>();

        npcSpawnPoints = new Transform[6];

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("spawn");

        for (int i = 0; i < spawnPoints.Length; i++)
            npcSpawnPoints[i] = spawnPoints[i].GetComponent<Transform>();

        GenerateFloor();
    }

    public void GenerateFloor()
    {
        ClearSpawnedNpcs();

        if (GameManager.Instance.currentPuzzle == null)
        {
            Debug.Log("Generating new puzzle...");
            GameManager.Instance.currentPuzzle = generator.Generate(GameManager.Instance.currentFloor, rng);
        }
        else
        {
            Debug.Log("Using existing puzzle...");
        }
        PuzzleData puzzle = GameManager.Instance.currentPuzzle;
       
        if (puzzle.npcInfo == null || puzzle.role == null)
        {
            Debug.Log("Loaded puzzle incomplete - regenerating...");
            GameManager.Instance.currentPuzzle = generator.Generate(GameManager.Instance.currentFloor, rng);
            puzzle = GameManager.Instance.currentPuzzle;
        }

        InitializeFloor(puzzle);
        DistributeAppearances(puzzle);
        SpawnNPCs(puzzle);
        ApplyAppearances(puzzle);
    }

    private void InitializeFloor(PuzzleData puzzle)
    {
        if (puzzle.npcInfo == null || puzzle.role == null)
        {
            Debug.LogError("Puzzle arrays are missing.");
            return;
        }

        if (puzzle.playerGuesses == null || puzzle.playerGuesses.Length != puzzle.npcCount)
        {
            puzzle.playerGuesses = new int[puzzle.npcCount];
            for (int i = 0; i < puzzle.playerGuesses.Length; i++)
                puzzle.playerGuesses[i] = 0;
        }

        for (int i = 0; i < puzzle.npcCount; i++)
        {
            if (string.IsNullOrEmpty(puzzle.npcInfo[i].label))
                puzzle.npcInfo[i].label = npcLabels[i];

            if (string.IsNullOrEmpty(puzzle.npcInfo[i].statementText) && puzzle.npcInfo[i].statement != null)
                puzzle.npcInfo[i].statementText = puzzle.npcInfo[i].statement.ToText(npcLabels, i);
        }
    }

    private void SpawnNPCs(PuzzleData puzzle)
    {
        if (npcSpawnPoints.Length < puzzle.npcCount)
        {
            Debug.LogError($"Not enough spawn points. Need {puzzle.npcCount}, have {npcSpawnPoints.Length}.");
            return;
        }

        List<Transform> availableSpawns = new List<Transform>(npcSpawnPoints);

        for (int i = 0; i < puzzle.npcCount; i++)
        {
            int index = rng.Next(0, availableSpawns.Count);
            Transform spawn = availableSpawns[index];

            GameObject npc = Instantiate(npcPrefab, spawn.position, spawn.rotation);

            NPCController controller = npc.GetComponent<NPCController>();
            controller.Setup(this, i, puzzle, statementController);

            availableSpawns.RemoveAt(index);
            spawnedNpcs.Add(controller);
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        List<int> available = new List<int>();
        for (int i = 0; i < npcSpawnPoints.Length; i++)
            if (!usedSpawnIndices.Contains(i))
                available.Add(i);

        if (available.Count == 0)
            available.Add(rng.Next(0, npcSpawnPoints.Length));

        int chosen = available[rng.Next(0, available.Count)];
        usedSpawnIndices.Add(chosen);
        return npcSpawnPoints[chosen];
    }

    private void DistributeAppearances(PuzzleData puzzle)
    {
        bool needAssignment = false;

        for (int i = 0; i < puzzle.npcCount; i++)
        {
            if (puzzle.npcInfo[i].headMaterialIndex < 0 || puzzle.npcInfo[i].bodyMaterialIndex < 0)
            {
                needAssignment = true;
                break;
            }
        }

        if (!needAssignment)
            return;

        List<Vector2Int> combos = new List<Vector2Int>();

        for (int h = 0; h < headMaterials.Length; h++)
        {
            for (int b = 0; b < bodyMaterials.Length; b++)
            {
                combos.Add(new Vector2Int(h, b));
            }
        }

        for (int i = 0; i < combos.Count; i++)
        {
            int j = rng.Next(i, combos.Count);
            Vector2Int temp = combos[i];
            combos[i] = combos[j];
            combos[j] = temp;
        }

        for (int i = 0; i < puzzle.npcCount; i++)
        {
            puzzle.npcInfo[i].headMaterialIndex = combos[i].x;
            puzzle.npcInfo[i].bodyMaterialIndex = combos[i].y;
        }
    }

    private void ApplyAppearances(PuzzleData puzzle)
    {
        Debug.Log("ApplyAppearances called. NPCs: " + spawnedNpcs.Count + ", Materials head: " + headMaterials.Length + ", body: " + bodyMaterials.Length);

        for (int i = 0; i < spawnedNpcs.Count && i < puzzle.npcCount; i++)
        {
            NPCAppearance appearance = spawnedNpcs[i].appearance;

            Debug.Log($"NPC {i} appearance component: " + (appearance == null ? "NULL" : "OK"));

            if (appearance == null)
                continue;

            int headIndex = puzzle.npcInfo[i].headMaterialIndex;
            int bodyIndex = puzzle.npcInfo[i].bodyMaterialIndex;

            Debug.Log($"NPC {i} materials - head: {headIndex}, body: {bodyIndex}");

            if (headIndex < 0 || headIndex >= headMaterials.Length ||
                bodyIndex < 0 || bodyIndex >= bodyMaterials.Length)
            {
                Debug.LogWarning($"Invalid material index on NPC {i}");
                continue;
            }

            appearance.Apply(headMaterials[headIndex], bodyMaterials[bodyIndex]);
        }
    }

    public void RevealStatement(int npcIndex)
    {
        if (currentPuzzle == null)
            return;

        if (npcIndex < 0 || npcIndex >= currentPuzzle.npcCount)
            return;

        if (currentPuzzle.npcInfo[npcIndex].discovered)
            return;

        currentPuzzle.npcInfo[npcIndex].discovered = true;
        answerPanelController.SetStatement(npcIndex, currentPuzzle.npcInfo[npcIndex].statementText);

        GameManager.Instance.SaveRun();
    }

    public void SubmitAnswers()
    {
        if (currentPuzzle == null)
            return;

        currentPuzzle.playerGuesses = answerPanelController.GetGuesses();
        GameManager.Instance.SaveRun();

        var solutions = PuzzleValidator.FindSolutions(currentPuzzle);
        Debug.Log($"Total solutions: {solutions.Count}");
        for (int s = 0; s < solutions.Count; s++)
        {
            string sol = "";
            for (int i = 0; i < solutions[s].Length; i++)
                sol += $"{currentPuzzle.npcInfo[i].label}={solutions[s][i]} ";
            Debug.Log($"Solution {s}: {sol}");
        }

        for (int i = 0; i < currentPuzzle.npcCount; i++)
        {
            string label = currentPuzzle.npcInfo[i].label;
            int guess = currentPuzzle.playerGuesses[i];
            int answer = (int)currentPuzzle.role[i];
            bool match = (guess == answer);
            Debug.Log($"{label}: guess={guess} ({Enum.GetName(typeof(Role), guess)}), answer={answer} ({Enum.GetName(typeof(Role), answer)}), match={match}");
        }

        Debug.Log("Current puzzle role array:");
        for (int i = 0; i < currentPuzzle.npcCount; i++)
        {
            Debug.Log($"  Index {i}: {currentPuzzle.role[i]} ({(Role)currentPuzzle.role[i]})");
        }
        Debug.Log("Player guesses array:");
        for (int i = 0; i < currentPuzzle.npcCount; i++)
        {
            int guess = currentPuzzle.playerGuesses[i];
            Debug.Log($"  Index {i}: {guess} ({(Role)guess})");
        }

        bool allCorrect = true;

        for (int i = 0; i < currentPuzzle.npcCount; i++)
        {
            if (currentPuzzle.playerGuesses[i] != (int)currentPuzzle.role[i])
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
            popupPanelController.ShowAnswerCorrect();
        else
            StartCoroutine(WrongSubmission());
    }

    private IEnumerator WrongSubmission()
    {
        bool dead = GameManager.Instance.DamagePlayer(1);

        if (dead)
        {
            RevealSolution();

            Debug.Log("Game Over - reveal current floor solution.");

            popupPanelController.ShowGameOver();
        }

        yield return null;
    }

    public IEnumerator FloorCleared(bool cleared)
    {
        if (screenFader != null)
            yield return screenFader.FadeOut(0.35f);

        DropItem();

        if (GameManager.Instance.currentFloor >= finalFloor)
        {
            Debug.Log("Victory!");

            popupPanelController.ShowGameClear();
        }
        else
        {
            if (cleared)
                GameManager.Instance.AdvanceFloor();

            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);

            if (screenFader != null)
                yield return screenFader.FadeIn(0.35f);
        }
    }

    private void RevealSolution()
    {
        answerPanelController.RevealAll(currentPuzzle.role);

        for (int i = 0; i < currentPuzzle.npcCount; i++)
        {
            Debug.Log($"{currentPuzzle.npcInfo[i].label} = {currentPuzzle.role[i]}");
        }
    }

    private void ClearSpawnedNpcs()
    {
        for (int i = 0; i < spawnedNpcs.Count; i++)
        {
            if (spawnedNpcs[i] != null)
                Destroy(spawnedNpcs[i].gameObject);
        }

        spawnedNpcs.Clear();
    }

    public int GetNPCCount()
    {
        return currentPuzzle.npcCount;
    }

    public int GetCurrentFloor()
    {
        return GameManager.Instance.currentFloor;
    }
}
