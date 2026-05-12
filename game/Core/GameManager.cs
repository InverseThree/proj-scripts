using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;

public class GameManager : MonoBehaviour
{
    public Flowchart flowchart;

    public static GameManager Instance;

    [Header("Run Settings")]
    public int baseMaxHealth = 3;

    public int currentFloor { get; private set; } = 1;
    public int currentHealth { get; private set; } = 3;
    public ItemType heldItem { get; private set; } = ItemType.None;

    public PuzzleData currentPuzzle;
    public RunInfoController runInfoController;
    public bool tutorialCompleted = false;

    public List<int> npcSpawnedIndices = new List<int>();

    public RunModifierState modifierState = new RunModifierState();

    public bool rewinded;

    public int currentMaxHealth => modifierState.currentMaxHealth;
    public RelicType heldRelic => modifierState.heldRelic;
    public bool IsHealthFull => currentHealth >= currentMaxHealth;

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
        flowchart = GameObject.FindGameObjectWithTag("flow").GetComponent<Flowchart>();
        runInfoController = FindObjectOfType<RunInfoController>();

        flowchart.SetBooleanVariable("tut", !tutorialCompleted);
        flowchart.SetIntegerVariable("floor", currentFloor);

        RefreshHUD();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartNewRun()
    {
        currentFloor = 0;
        currentHealth = baseMaxHealth;
        heldItem = ItemType.None;
        currentPuzzle = null;
        modifierState.ResetForNewRun(baseMaxHealth);

        SaveRun();
    }

    public bool TryLoadRun()
    {
        RunSaveData data = SaveSystem.Load();
        if (data == null)
            return false;

        currentFloor = data.currentFloor;
        currentHealth = data.currentHealth;
        heldItem = data.heldItem;
        currentPuzzle = data.currentPuzzle;
        modifierState = data.modifierState ?? new RunModifierState();

        RefreshHUD();
        return true;
    }

    public void SaveRun()
    {
        RunSaveData data = new RunSaveData
        {
            currentFloor = currentFloor,
            currentHealth = currentHealth,
            heldItem = heldItem,
            modifierState = modifierState,
            currentPuzzle = currentPuzzle
        };

        SaveSystem.Save(data);
    }

    public void RefreshHUD()
    {
        PuzzleData.FloorSpecialState floorState = EnsureFloorState();

        if (runInfoController != null)
            runInfoController.RefreshAll();

        if (modifierState.talismanActive && !floorState.talismanPrevented)
            runInfoController.SetBarrier(true);

        if (floorState.shieldActive)
            runInfoController.SetBarrier(true);
    }

    public void GiveItem(ItemType item)
    {
        heldItem = item;
        SaveRun();
        RefreshHUD();
    }

    public void ClearItem()
    {
        heldItem = ItemType.None;
        SaveRun();
        RefreshHUD();
    }

    public void GiveRelic(RelicType relic)
    {
        if (modifierState.heldRelic != RelicType.None)
            return;

        modifierState.heldRelic = relic;

        switch (relic)
        {
            case RelicType.Talisman:
                modifierState.currentMaxHealth = 1;
                currentHealth = Mathf.Min(currentHealth, 1);
                break;

            case RelicType.Scythe:
                heldItem = ItemType.None;
                break;
        }

        SaveRun();
        RefreshHUD();
    }

    public bool HealPlayer(int amount)
    {
        if (currentHealth >= currentMaxHealth)
            return false;

        currentHealth = Mathf.Min(currentMaxHealth, currentHealth + amount);
        SaveRun();
        RefreshHUD();
        return true;
    }

    public int GetFinalFloor()
    {
        return modifierState.brushActive ? 15 : 10;
    }

    public int GetDamageAmount(int wrongGuessCount)
    {
        if (modifierState.shardActive)
            return wrongGuessCount;

        return wrongGuessCount > 0 ? 1 : 0;
    }

    public bool TryTakeDamage(int amount, out bool prevented, out bool revived)
    {
        prevented = false;
        revived = false;

        if (amount <= 0)
            return currentHealth <= 0;

        PuzzleData.FloorSpecialState floorState = EnsureFloorState();

        if (floorState.shieldActive)
        {
            floorState.shieldActive= false;
            prevented = true;
            runInfoController.SetBarrier(false);
            SaveRun();
            RefreshHUD();
            return false;
        }

        if (modifierState.talismanActive && !floorState.talismanPrevented)
        {
            floorState.talismanPrevented = true;
            prevented = true;
            runInfoController.SetBarrier(false);
            SaveRun();
            RefreshHUD();
            return false;
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);

        if (currentHealth <= 0 && modifierState.coinActive && !modifierState.coinRevived)
        {
            modifierState.coinRevived = true;
            currentHealth = currentMaxHealth;
            revived = true;
        }

        SaveRun();
        RefreshHUD();
        return currentHealth <= 0;
    }

    public PuzzleData.FloorSpecialState EnsureFloorState()
    {
        if (currentPuzzle == null)
            currentPuzzle = new PuzzleData();

        if (currentPuzzle.floorState == null)
            currentPuzzle.floorState = new PuzzleData.FloorSpecialState();

        if (currentPuzzle.hints == null)
            currentPuzzle.hints = new System.Collections.Generic.List<string>();

        return currentPuzzle.floorState;
    }

    public void AdvanceFloor()
    {
        currentFloor++;
        currentPuzzle = null;
        npcSpawnedIndices.Clear();
        SaveRun();
    }

    public void FinishTut()
    {
        GameManager.Instance.tutorialCompleted = true;
    }
}
