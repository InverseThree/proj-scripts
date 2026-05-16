using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;

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
    public RewardPanelController rewardPanelController;
    public HintLogController hintLogController;
    public ScythePanelController scythePanelController;
    public LampPanelController lampPanelController;

    [Header("Flow")]
    public ScreenFader screenFader;

    public List<NPCController> spawnedNPCs = new List<NPCController>();
    private System.Random rng = new System.Random();
    private PuzzleGenerator generator = new PuzzleGenerator();

    private PuzzleData currentPuzzle => GameManager.Instance.currentPuzzle;

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
        popupPanelController = FindObjectOfType<PopupPanelController>();
        rewardPanelController = FindObjectOfType<RewardPanelController>();
        hintLogController = FindObjectOfType<HintLogController>();
        scythePanelController = FindObjectOfType<ScythePanelController>();
        lampPanelController = FindObjectOfType<LampPanelController>();
        screenFader = FindObjectOfType<ScreenFader>();

        GameObject spawnPoints = GameObject.FindGameObjectWithTag("spawn");
        npcSpawnPoints = new Transform[6];
        for (int i = 0; i < 6; i++)
            npcSpawnPoints[i] = spawnPoints.transform.Find($"spawn{i}");

        GenerateFloor();
    }

    public void GenerateFloor()
    {
		ClearSpawnedNPCs();

		GameManager.Instance.currentPuzzle = generator.Generate(GameManager.Instance.currentFloor, rng, false, null);
		PuzzleData puzzle = GameManager.Instance.currentPuzzle;
		DistributeAppearances(puzzle);
		SpawnNPCs(puzzle);
		ApplyAppearances(puzzle);
		InitializeFloor(puzzle);

		ModifyDisplayText(puzzle);
		hintLogController.BuildFromPuzzle(puzzle);

		if (GameManager.Instance.modifierState.shardActive && !currentPuzzle.floorState.shardResolved && !GameManager.Instance.rewinded) 
			StartCoroutine(ShardActivate());

		GameManager.Instance.rewinded = false;

        GameManager.Instance.SaveRun();
    }

    private void InitializeFloor(PuzzleData puzzle)
    {
        if (puzzle.playerGuesses == null || puzzle.playerGuesses.Length != puzzle.npcCount)
        {
            puzzle.playerGuesses = new int[puzzle.npcCount];
            for (int i = 0; i < puzzle.npcCount; i++)
                puzzle.playerGuesses[i] = 0;
        }

        if (puzzle.floorState == null)
            puzzle.floorState = new PuzzleData.FloorSpecialState();

        if (puzzle.hints == null)
            puzzle.hints = new List<string>();

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

		int[] index = new int[puzzle.npcCount];

        List<Transform> availableSpawns = new List<Transform>(npcSpawnPoints);

        for (int i = 0; i < puzzle.npcCount; i++)
        {
			Transform spawn;
			int spawnedIndex = -1;

            index[i] = rng.Next(0, availableSpawns.Count);

			if (!GameManager.Instance.currentPuzzle.floorState.scytheUsed)
			{
				GameManager.Instance.npcSpawnedIndices.Add(index[i]);
				spawn = availableSpawns[index[i]];

                for (int j = 0; j < npcSpawnPoints.Length; j++)
                {
                    if (spawn == npcSpawnPoints[j])
                    {
                        spawnedIndex = j;
                        break;
                    }
                }
			}
			else
				spawn = availableSpawns[GameManager.Instance.npcSpawnedIndices[i]];

            GameObject npc = Instantiate(npcPrefab, spawn.position, spawn.rotation);
			Debug.Log($"Npc{i}");

            NPCController controller = npc.GetComponent<NPCController>();

            controller.Setup(this, i, spawnedIndex, puzzle, statementController);

            availableSpawns.RemoveAt(GameManager.Instance.currentPuzzle.floorState.scytheUsed ? GameManager.Instance.npcSpawnedIndices[i] : index[i]);
            spawnedNPCs.Add(controller);
        }

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

        // Shuffle combinations
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
        Debug.Log("ApplyAppearances called. NPCs: " + spawnedNPCs.Count + ", Materials head: " + headMaterials.Length + ", body: " + bodyMaterials.Length);

        for (int i = 0; i < spawnedNPCs.Count && i < puzzle.npcCount; i++)
        {
            NPCAppearance appearance = spawnedNPCs[i].appearance;

            Debug.Log($"NPC {i} appearance component: " + (appearance == null ? "NULL" : "OK"));

            if (appearance == null)
                continue;

            int headIndex = puzzle.npcInfo[i].headMaterialIndex;
            int bodyIndex = puzzle.npcInfo[i].bodyMaterialIndex;

            Debug.Log($"NPC {i} materials - head: {headIndex}, body: {bodyIndex}");

            if (headIndex < 0 || headIndex >= headMaterials.Length || bodyIndex < 0 || bodyIndex >= bodyMaterials.Length)
            {
                Debug.LogWarning($"Invalid material index on NPC {i}");
                continue;
            }

            appearance.Apply(headMaterials[headIndex], bodyMaterials[bodyIndex]);
        }
    }

    private void ModifyDisplayText(PuzzleData puzzle)
    {
        if (GameManager.Instance.modifierState.lampActive && puzzle.floorState.lampHiddenIndex < 0)
        {
            List<int> candidates = new List<int>();
            for (int i = 0; i < puzzle.npcCount; i++)
            {
                if (puzzle.npcInfo[i].statement.GetReferencedIndices(i).Count > 0)
                    candidates.Add(i);
            }

            if (candidates.Count > 0)
                puzzle.floorState.lampHiddenIndex = candidates[rng.Next(candidates.Count)];
        }

        for (int i = 0; i < puzzle.npcCount; i++)
        {
            string[] names = (string[])npcLabels.Clone();

            if (GameManager.Instance.modifierState.lampActive && puzzle.floorState.lampHiddenIndex == i)
            {
                List<int> refs = puzzle.npcInfo[i].statement.GetReferencedIndices(i);
                for (int r = 0; r < refs.Count; r++)
                    names[refs[r]] = "?";
            }

            puzzle.npcInfo[i].statementText = puzzle.npcInfo[i].statement.ToText(names, i);
            spawnedNPCs[i].statementText = puzzle.npcInfo[i].statementText;
        }

        if (GameManager.Instance.modifierState.mirrorPending && puzzle.floorState.mirrorHintA < 0)
        {
            List<(int, int)> samePairs = new List<(int, int)>();

            for (int i = 0; i < puzzle.npcCount; i++)
            {
                for (int j = i + 1; j < puzzle.npcCount; j++)
                {
                    if (puzzle.role[i] == puzzle.role[j])
                        samePairs.Add((i, j));
                }
            }

            if (samePairs.Count > 0)
            {
                var pair = samePairs[rng.Next(samePairs.Count)];
                puzzle.floorState.mirrorHintA = pair.Item1;
                puzzle.floorState.mirrorHintB = pair.Item2;

                AddHint($"Mirror of Truth: {puzzle.npcInfo[pair.Item1].label} and {puzzle.npcInfo[pair.Item2].label} have the same identity.");
            }
			else
                AddHint("Mirror of Truth: None of the inhabitants on this floor have the same identity.");

            GameManager.Instance.modifierState.mirrorPending = false;
        }
    }

    private IEnumerator ShardActivate()
    {
        do
            yield return null;
        while (SayDialog.GetSayDialog().isActiveAndEnabled || MenuDialog.GetMenuDialog().isActiveAndEnabled);

		List<ItemType> items = GetItemChoice(3, excludeMirrorOnFinalFloor: GameManager.Instance.currentFloor >= GameManager.Instance.GetFinalFloor());

		bool done = false;
		rewardPanelController.ShowItemChoice(items, GameManager.Instance.heldItem != ItemType.None, selected =>
			{
				if (selected != ItemType.None)
				    GameManager.Instance.GiveItem(selected);

				currentPuzzle.floorState.shardResolved = true;
				GameManager.Instance.SaveRun();
				done = true;
			});

		yield return new WaitUntil(() => done);
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

        int wrongCount = 0;
        for (int i = 0; i < currentPuzzle.npcCount; i++)
        {
            if (currentPuzzle.playerGuesses[i] != (int)currentPuzzle.role[i])
                wrongCount++;
        }

        if (wrongCount == 0)
        {
			if (GameManager.Instance.currentFloor == GameManager.Instance.GetFinalFloor())
			{
				popupPanelController.ShowGameClear();
				return;
			}
			else
			{
				popupPanelController.ShowAnswerCorrect();
				return;
			}
        }

        int damage = GameManager.Instance.GetDamageAmount(wrongCount);
        StartCoroutine(WrongSubmission(damage));
    }

    private IEnumerator WrongSubmission(int damage)
    {
        bool dead = GameManager.Instance.TryTakeDamage(damage, out _, out _);

        if (dead)
        {
            RevealSolution();
            popupPanelController.ShowGameOver();
        }

        yield return null;
    }

    public IEnumerator ResolveFloorClear()
    {
        int clearedFloor = GameManager.Instance.currentFloor;

        if (clearedFloor == 5 && GameManager.Instance.heldRelic == RelicType.None)
        {
            bool relicDone = false;

			popupPanelController.gameObject.SetActive(false);
            rewardPanelController.ShowRelicChoice(GetRelicChoice(3), relic =>
            {
                if (relic != RelicType.None)
                    GameManager.Instance.GiveRelic(relic);

                relicDone = true;
            });

            yield return new WaitUntil(() => relicDone);
        }

        if (clearedFloor % 3 == 0 && clearedFloor != 0)
        {
            ItemType rewardItem = GetRandomItem(excludeMirrorOnFinalFloor: GameManager.Instance.currentFloor >= GameManager.Instance.GetFinalFloor());

            bool itemDone = false;

			popupPanelController.gameObject.SetActive(false);
            rewardPanelController.ShowItemOffer(rewardItem, !GameManager.Instance.modifierState.itemSlotDisabled, GameManager.Instance.heldItem != ItemType.None, decision =>
                {
                    if (decision == ItemOptions.Take)
                    {
                        GameManager.Instance.GiveItem(rewardItem);
                    }
                    else if (decision == ItemOptions.TakeAndUse)
                    {
                        GameManager.Instance.GiveItem(rewardItem);
                        TryUseHeldItem();
                    }

                    itemDone = true;
                });

            yield return new WaitUntil(() => itemDone);
        }

        GameManager.Instance.AdvanceFloor();
		StartCoroutine(LoadFloor());
    }

    public void OnItemSlotClicked()
    {
        if (GameManager.Instance.heldItem == ItemType.None)
            return;

        rewardPanelController.ShowUseItemPrompt(GameManager.Instance.heldItem, confirmed =>
            {
                if (confirmed)
                    TryUseHeldItem();
            });
    }

    public void OnRelicSlotClicked()
    {
        switch (GameManager.Instance.heldRelic)
        {
            case RelicType.Scythe:
                if (currentPuzzle.floorState.scytheUsed)
                {
                    rewardPanelController.ShowMessage(false, "Held Relic", "This relic has already been used on this floor.");
                    return;
                }

                scythePanelController.Show(selected =>
                    {
                        if (selected.Count == 0)
						    return;

                        bool success = generator.RerollSelected(currentPuzzle, this, selected, rng);
                        if (success)
                        {
						    for (int i = 0; i < currentPuzzle.npcCount; i++)
						    {
						        Debug.Log($"POST-REROLL ROLE {i}: {currentPuzzle.role[i]}");
						    }
						    currentPuzzle.floorState.scytheUsed = true;

						    currentPuzzle.hints.Clear();
						    currentPuzzle.floorState.ClearDerivedFloorHints();

						    ClearSpawnedNPCs();

						    SpawnNPCs(currentPuzzle);
						    DistributeAppearances(currentPuzzle);
						    ApplyAppearances(currentPuzzle);

						    answerPanelController.BuildFromPuzzle(GameManager.Instance.currentPuzzle);
						    hintLogController.BuildFromPuzzle(GameManager.Instance.currentPuzzle);

						    GameManager.Instance.SaveRun();
						}
                    });
                break;

            case RelicType.Lamp:
                if (GameManager.Instance.modifierState.lampTotalUsed >= 3)
                {
                    rewardPanelController.ShowMessage(false, "Held Relic", "No wishes remain.");
                    break;
                }

                lampPanelController.Show(currentPuzzle.npcCount, npcLabels, GameManager.Instance.modifierState, request =>
                    {
                        MakeWish(request);
                    });
                break;

            default:
                if (GameManager.Instance.heldRelic != RelicType.None)
                rewardPanelController.ShowMessage(false, "Held Relic", "This relic has no active ability.");
                break;
        }
    }

    public bool TryUseHeldItem()
    {
        switch (GameManager.Instance.heldItem)
        {
            case ItemType.None:
                return false;

            case ItemType.Tonic:
                if (!GameManager.Instance.HealPlayer(1))
                    return false;

                GameManager.Instance.ClearItem();
                return true;

            case ItemType.Shield:
                currentPuzzle.floorState.shieldActive = true;
                GameManager.Instance.ClearItem();
                GameManager.Instance.SaveRun();
                return true;

            case ItemType.Lens:
                if (GameManager.Instance.TryTakeDamage(1, out _, out _))
                {
                    RevealSolution();
                    popupPanelController.ShowGameOver();
                }
                RevealRandomIdentity();
                GameManager.Instance.ClearItem();
                return true;

            case ItemType.Scroll:
                RevealRandomCount();
                GameManager.Instance.ClearItem();
                return true;

            case ItemType.Mirror:
                GameManager.Instance.modifierState.mirrorPending = true;
                GameManager.Instance.ClearItem();
                GameManager.Instance.SaveRun();
                return true;

            case ItemType.Hourglass:
                StartCoroutine(Rewind());
                GameManager.Instance.ClearItem();
                return true;
        }

        return false;
    }

    private void RevealRandomIdentity()
    {
        List<int> candidates = new List<int>();

        for (int i = 0; i < currentPuzzle.npcCount; i++)
        {
            if (!currentPuzzle.npcInfo[i].identityRevealed)
                candidates.Add(i);
        }

        if (candidates.Count == 0)
        {
            rewardPanelController.ShowMessage(true, "Held Item", "No valid inhabitant is left to reveal.");
            return;
        }

        int pickedIndex = candidates[rng.Next(candidates.Count)];
        currentPuzzle.npcInfo[pickedIndex].identityRevealed = true;
        answerPanelController.RevealIdentity(currentPuzzle.npcInfo[pickedIndex].label, currentPuzzle.role[pickedIndex]);
        AddHint($"Lens of Devil: {currentPuzzle.npcInfo[pickedIndex].label} is a {currentPuzzle.role[pickedIndex].ToString().ToLower()}.");
        GameManager.Instance.SaveRun();
    }

    private void RevealRandomCount()
    {
        int knightCount = 0;
        int knaveCount = 0;

        for (int i = 0; i < currentPuzzle.npcCount; i++)
        {
            if (currentPuzzle.role[i] == Role.Knight) knightCount++;
            if (currentPuzzle.role[i] == Role.Knave) knaveCount++;
        }

        bool revealKnights = rng.NextDouble() < 0.5;
        currentPuzzle.floorState.scrollViewed = true;
        currentPuzzle.floorState.scrollRevealRole = revealKnights;
        currentPuzzle.floorState.scrollRevealNumber = revealKnights ? knightCount : knaveCount;

        AddHint($"Scroll of Insight: There are either {currentPuzzle.floorState.scrollRevealNumber} knights or {currentPuzzle.floorState.scrollRevealNumber} knaves on this floor.");
        GameManager.Instance.SaveRun();
    }

    private IEnumerator Rewind()
    {
        GameManager.Instance.modifierState.lampTotalUsed -= currentPuzzle.floorState.lampWishesSpent;
        GameManager.Instance.modifierState.lampIdentityTotalUsed -= currentPuzzle.floorState.lampIdentitySpent;
        GameManager.Instance.modifierState.lampPairTotalUsed -= currentPuzzle.floorState.lampPairSpent;
        GameManager.Instance.modifierState.lampCountTotalUsed -= currentPuzzle.floorState.lampCountSpent;

        GameManager.Instance.modifierState.lampTotalUsed = Mathf.Max(0, GameManager.Instance.modifierState.lampTotalUsed);
        GameManager.Instance.modifierState.lampIdentityTotalUsed = Mathf.Max(0, GameManager.Instance.modifierState.lampIdentityTotalUsed);
        GameManager.Instance.modifierState.lampPairTotalUsed = Mathf.Max(0, GameManager.Instance.modifierState.lampPairTotalUsed);
        GameManager.Instance.modifierState.lampCountTotalUsed = Mathf.Max(0, GameManager.Instance.modifierState.lampCountTotalUsed);

        bool mirrorUsed = currentPuzzle.floorState.mirrorHintA >= 0 && currentPuzzle.floorState.mirrorHintB >= 0;

        if (mirrorUsed)
            GameManager.Instance.modifierState.mirrorPending = true;
		bool retakeDamage = false;
		if (currentPuzzle.floorState.talismanPrevented == true)
			retakeDamage = true;

		GameManager.Instance.rewinded = true;
		GameManager.Instance.SaveRun();

		yield return StartCoroutine(LoadFloor());

        answerPanelController.BuildFromPuzzle(GameManager.Instance.currentPuzzle);

		if (retakeDamage)
		{
			currentPuzzle.floorState.talismanPrevented = false;
			GameManager.Instance.TryTakeDamage(1, out _, out _);
		}
    }

    private void MakeWish(LampPanelController.LampWishRequest request)
    {
        RunModifierState mods = GameManager.Instance.modifierState;
        PuzzleData.FloorSpecialState state = currentPuzzle.floorState;

        switch (request.type)
        {
            case LampWishType.Identity:
                if (mods.lampIdentityTotalUsed >= 1)
                    return;

                mods.lampTotalUsed++;
                mods.lampIdentityTotalUsed++;
                state.lampWishesSpent++;
                state.lampIdentitySpent++;
                state.lampIdentityIndex = request.a;

                currentPuzzle.npcInfo[request.a].identityRevealed = true;
                answerPanelController.RevealIdentity(currentPuzzle.npcInfo[request.a].label, currentPuzzle.role[request.a]);
                AddHint($"Lamp of Oracle: {currentPuzzle.npcInfo[request.a].label} is a {currentPuzzle.role[request.a].ToString().ToLower()}.");
                break;

            case LampWishType.Pair:
                if (mods.lampPairTotalUsed >= 2)
                    return;

                mods.lampTotalUsed++;
                mods.lampPairTotalUsed++;
                state.lampWishesSpent++;
                state.lampPairSpent++;

                bool same = currentPuzzle.role[request.a] == currentPuzzle.role[request.b];
                state.lampPairHints.Add(new PuzzleData.LampPairHint { a = request.a, b = request.b, same = same });

                AddHint($"Lamp of Oracle: {currentPuzzle.npcInfo[request.a].label} and {currentPuzzle.npcInfo[request.b].label} {(same ? "have" : "do not have")} the same identity.");
                break;

            case LampWishType.Counts:
                if (mods.lampCountTotalUsed >= 3)
                    return;

                mods.lampTotalUsed++;
                mods.lampCountTotalUsed++;
                state.lampWishesSpent++;
                state.lampCountSpent++;

                int knightCount = 0;
                int knaveCount = 0;
                for (int i = 0; i < currentPuzzle.npcCount; i++)
                {
                    if (currentPuzzle.role[i] == Role.Knight)
						knightCount++;
					else if (currentPuzzle.role[i] == Role.Knave)
						knaveCount++;
                }

                state.lampCountGranted = true;
                state.lampKnightCount = knightCount;
                state.lampKnaveCount = knaveCount;

                AddHint($"Lamp of Oracle: There are {knightCount} knights and {knaveCount} knaves on this floor.");
                break;
        }

        GameManager.Instance.SaveRun();
    }

    private void AddHint(string text)
    {
        if (currentPuzzle.hints == null)
            currentPuzzle.hints = new List<string>();

        currentPuzzle.hints.Add(text);
        hintLogController.BuildFromPuzzle(currentPuzzle);
    }

    private ItemType GetRandomItem(bool excludeMirrorOnFinalFloor)
    {
        List<ItemType> pool = new List<ItemType>
        {
            ItemType.Tonic,
            ItemType.Shield,
            ItemType.Lens,
            ItemType.Scroll,
            ItemType.Mirror,
            ItemType.Hourglass
        };

        if (excludeMirrorOnFinalFloor)
            pool.Remove(ItemType.Mirror);

        return pool[rng.Next(pool.Count)];
    }

    private List<ItemType> GetItemChoice(int count, bool excludeMirrorOnFinalFloor)
    {
        List<ItemType> pool = new List<ItemType>
        {
            ItemType.Tonic,
            ItemType.Shield,
            ItemType.Lens,
            ItemType.Scroll,
            ItemType.Mirror,
            ItemType.Hourglass
        };

        if (excludeMirrorOnFinalFloor)
            pool.Remove(ItemType.Mirror);

		return AddReward<ItemType>(pool, count);
    }

    private List<RelicType> GetRelicChoice(int count)
    {
        List<RelicType> pool = new List<RelicType>
        {
            RelicType.Coin,
            RelicType.Brush,
            RelicType.Talisman,
            RelicType.Shard,
            RelicType.Scythe,
            RelicType.Lamp
        };

		return AddReward<RelicType>(pool, count);
    }

	private List<T> AddReward<T>(List<T> pool, int count)
	{
		List<T> list = new List<T>();

		while (list.Count < count && pool.Count > 0)
		{
			int idx = rng.Next(pool.Count);
			list.Add(pool[idx]);
			pool.RemoveAt(idx);
		}

		return list;
	}

    private void RevealSolution()
    {
        for (int i = 0; i < currentPuzzle.npcCount; i++)
            RevealStatement(i);
        answerPanelController.RevealAll(currentPuzzle.role);
    }

    private void ClearSpawnedNPCs()
    {
        for (int i = 0; i < spawnedNPCs.Count; i++)
        {
            if (spawnedNPCs[i] != null)
                Destroy(spawnedNPCs[i].gameObject);
        }

        spawnedNPCs.Clear();
    }

    public int GetNPCCount()
    {
        return currentPuzzle.npcCount;
    }

    public int GetCurrentFloor()
    {
        return GameManager.Instance.currentFloor;
    }

	private IEnumerator LoadFloor()
	{
        if (screenFader != null)
            yield return screenFader.FadeOut(0.35f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
