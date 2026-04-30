using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Run Settings")]
    public int maxHealth = 3;

    public int currentFloor { get; private set; } = 1;
    public int currentHealth { get; private set; } = 3;
    public ItemType heldItem { get; private set; } = ItemType.None;

    public PuzzleData currentPuzzle;
    public RunInfoController runInfoController;

    public bool tutorialCompleted = false;

    public bool IsHealthFull => currentHealth >= maxHealth;

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
        runInfoController = FindObjectOfType<RunInfoController>();

        Flowchart flowchart = FindObjectOfType<Flowchart>();
        flowchart.SetBooleanVariable("tut", !GameManager.Instance.tutorialCompleted);
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
        currentFloor = 1;
        currentHealth = maxHealth;
        heldItem = ItemType.None;
        currentPuzzle = null;

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

        return true;
    }

    public void SaveRun()
    {
        RunSaveData data = new RunSaveData
        {
            currentFloor = currentFloor,
            currentHealth = currentHealth,
            heldItem = heldItem,
            currentPuzzle = currentPuzzle
        };

        SaveSystem.Save(data);
    }

    public bool DamagePlayer(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);

        runInfoController.ChangeHealth(true);
        SaveRun();

        return currentHealth <= 0;
    }

    public void AdvanceFloor()
    {
        currentFloor++;
        currentPuzzle = null;
        SaveRun();
    }

    public void FinishTut()
    {
        GameManager.Instance.tutorialCompleted = true;
    }
}
