using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class LeaderboardView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private LeaderboardEntry entryPrefab;

    [SerializeField]
    private UpdateButton updateButtonPrefab;

    [SerializeField]
    private int visibleEntries = 10;

    [SerializeField]
    private float entrySpacing = 1.4f;

    [SerializeField]
    private float animationDuration = 0.6f;

    [Header("Layout")]
    [SerializeField]
    private Transform entriesContainer;

    [SerializeField]
    private Transform uiContainer;

    [Header("Background")]
    [SerializeField]
    private SpriteRenderer backgroundRenderer;

    private ObjectPool<LeaderboardEntry> entryPool;
    private List<LeaderboardEntry> activeEntries = new List<LeaderboardEntry>();
    private List<PlayerData> currentPlayers = new List<PlayerData>();

    private UpdateButton updateButton;
    private LeaderboardEntry currentPlayerEntry;
    private int currentPlayerIndex;
    private bool isAnimating = false;

    // Better position tracking
    private Dictionary<int, Vector3> slotPositions = new Dictionary<int, Vector3>();

    public static LeaderboardView Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetupLeaderboard();
    }

    private void SetupLeaderboard()
    {
        CreateContainers();
        SetupBackground();
        InitializePool();
        CreateUpdateButton();
        StartCoroutine(WaitForDataAndDisplay());
    }

    private void CreateContainers()
    {
        if (entriesContainer == null)
        {
            GameObject container = new GameObject("EntriesContainer");
            container.transform.SetParent(transform);
            entriesContainer = container.transform;
        }

        if (uiContainer == null)
        {
            GameObject container = new GameObject("UIContainer");
            container.transform.SetParent(transform);
            uiContainer = container.transform;
        }
    }

    private void SetupBackground()
    {
        if (backgroundRenderer == null)
        {
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(transform);
            backgroundRenderer = bgObj.AddComponent<SpriteRenderer>();
        }

        // Clash of Clans style background
        backgroundRenderer.color = new Color(0.15f, 0.25f, 0.4f, 0.9f);
        backgroundRenderer.transform.localScale = new Vector3(14f, 18f, 1f);
        backgroundRenderer.sortingOrder = -1;
    }

    private void InitializePool()
    {
        entryPool = new ObjectPool<LeaderboardEntry>(
            entryPrefab,
            entriesContainer,
            visibleEntries + 15
        );
    }

    private void CreateUpdateButton()
    {
        updateButton = Instantiate(updateButtonPrefab, uiContainer);
        updateButton.transform.position = new Vector3(0, 8f, 0);
        updateButton.OnButtonClicked += OnUpdateButtonClicked;
    }

    private IEnumerator WaitForDataAndDisplay()
    {
        yield return new WaitUntil(() => DataManager.Instance != null);
        DisplayLeaderboard();
    }

    public void DisplayLeaderboard()
    {
        var sortedPlayers = DataManager.Instance.GetSortedPlayers();
        currentPlayers = new List<PlayerData>(sortedPlayers);
        currentPlayerIndex = currentPlayers.FindIndex(p => p.IsCurrentPlayer);

        CalculateSlotPositions();
        DisplayVisibleEntries();
    }

    private void CalculateSlotPositions()
    {
        slotPositions.Clear();

        for (int i = 0; i < visibleEntries; i++)
        {
            float yPosition = (visibleEntries / 2f - i - 0.5f) * entrySpacing;
            slotPositions[i] = new Vector3(0, yPosition, 0);
        }
    }

    private void DisplayVisibleEntries()
    {
        // Clear all existing entries
        entryPool.ReturnAll(activeEntries);
        activeEntries.Clear();

        // Calculate visible range
        int startIndex = CalculateStartIndex(currentPlayerIndex);
        int endIndex = Mathf.Min(currentPlayers.Count, startIndex + visibleEntries);

        // Create entries for visible range
        for (int i = startIndex; i < endIndex; i++)
        {
            CreateEntryAtSlot(i, i - startIndex);
        }
    }

    private int CalculateStartIndex(int playerIndex)
    {
        int startIndex = Mathf.Max(0, playerIndex - visibleEntries / 2);
        int maxStartIndex = Mathf.Max(0, currentPlayers.Count - visibleEntries);
        return Mathf.Min(startIndex, maxStartIndex);
    }

    private void CreateEntryAtSlot(int playerIndex, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= visibleEntries)
            return;

        var entry = entryPool.Get();
        var player = currentPlayers[playerIndex];
        bool isCurrentPlayer = player.IsCurrentPlayer;

        entry.SetupEntry(player, isCurrentPlayer);
        entry.transform.position = slotPositions[slotIndex];

        activeEntries.Add(entry);

        if (isCurrentPlayer)
        {
            currentPlayerEntry = entry;
        }

        entry.PlayEntryAnimation();
    }

    private void OnUpdateButtonClicked()
    {
        if (isAnimating)
            return;
        StartCoroutine(UpdateLeaderboard());
    }

    private IEnumerator UpdateLeaderboard()
    {
        isAnimating = true;
        updateButton.SetInteractable(false);

        // Get updated data
        DataManager.Instance.UpdateScoresRandomly();
        var newSortedPlayers = DataManager.Instance.GetSortedPlayers();
        int newPlayerIndex = newSortedPlayers.FindIndex(p => p.IsCurrentPlayer);
        int newStartIndex = CalculateStartIndex(newPlayerIndex);

        // Perform the update
        yield return StartCoroutine(
            PerformLeaderboardUpdate(newSortedPlayers, newPlayerIndex, newStartIndex)
        );

        // Update state
        currentPlayers = new List<PlayerData>(newSortedPlayers);
        currentPlayerIndex = newPlayerIndex;

        isAnimating = false;
        updateButton.SetInteractable(true);
    }

    private IEnumerator PerformLeaderboardUpdate(
        List<PlayerData> newPlayers,
        int newPlayerIndex,
        int newStartIndex
    )
    {
        // Create mapping of old entries by player ID
        var entryMap = new Dictionary<string, LeaderboardEntry>();
        foreach (var entry in activeEntries)
        {
            if (entry.PlayerData != null)
            {
                entryMap[entry.PlayerData.id] = entry;
            }
        }

        // Clear active entries list but keep the objects
        activeEntries.Clear();

        // Calculate new visible range
        int endIndex = Mathf.Min(newPlayers.Count, newStartIndex + visibleEntries);

        // Phase 1: Update and position all entries
        var animations = new List<Tween>();

        for (int i = newStartIndex; i < endIndex; i++)
        {
            var player = newPlayers[i];
            int slotIndex = i - newStartIndex;
            Vector3 targetPosition = slotPositions[slotIndex];

            LeaderboardEntry entry;

            if (entryMap.ContainsKey(player.id))
            {
                // Reuse existing entry
                entry = entryMap[player.id];
                entry.UpdateData(player);
                entryMap.Remove(player.id); // Remove from map so we know it's used
            }
            else
            {
                // Create new entry
                entry = entryPool.Get();
                entry.SetupEntry(player, player.IsCurrentPlayer);
            }

            activeEntries.Add(entry);

            if (player.IsCurrentPlayer)
            {
                currentPlayerEntry = entry;
            }

            // Animate to target position
            if (Vector3.Distance(entry.transform.position, targetPosition) > 0.1f)
            {
                var tween = entry.AnimateToPosition(targetPosition, animationDuration);
                animations.Add(tween);
            }
            else
            {
                entry.transform.position = targetPosition;
            }
        }

        // Return unused entries to pool
        foreach (var unusedEntry in entryMap.Values)
        {
            entryPool.Return(unusedEntry);
        }

        // Phase 2: Highlight current player if they moved
        if (currentPlayerEntry != null)
        {
            currentPlayerEntry.PlayHighlightAnimation();
        }

        // Wait for animations to complete
        if (animations.Count > 0)
        {
            yield return new WaitForSeconds(animationDuration);
        }

        // Ensure all positions are exact
        for (int i = 0; i < activeEntries.Count; i++)
        {
            if (i < slotPositions.Count)
            {
                activeEntries[i].transform.position = slotPositions[i];
            }
        }

        yield return null;
    }
}
