using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [Header("Data Settings")]
    [SerializeField]
    private string jsonFileName = "tournament_data.json";

    [SerializeField]
    private bool useStreamingAssets = true;

    [SerializeField]
    private TextAsset jsonFile;

    private TournamentData tournamentData;
    private List<PlayerData> originalPlayers;

    public static DataManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadTournamentData();
    }

    private void LoadTournamentData()
    {
        string jsonString = null;

        if (useStreamingAssets)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);

            if (File.Exists(filePath))
            {
                jsonString = File.ReadAllText(filePath);
                Debug.Log($"Loaded tournament data from StreamingAssets: {filePath}");
            }
            else
            {
                Debug.LogWarning($"JSON file not found at: {filePath}");
            }
        }
        else if (jsonFile != null)
        {
            jsonString = jsonFile.text;
            Debug.Log("Loaded tournament data from TextAsset");
        }

        if (!string.IsNullOrEmpty(jsonString))
        {
            try
            {
                tournamentData = JsonUtility.FromJson<TournamentData>(jsonString);
                Debug.Log($"Successfully parsed {tournamentData.players.Count} players from JSON");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to parse JSON: {e.Message}");
                tournamentData = null;
            }
        }

        if (tournamentData == null || tournamentData.players.Count == 0)
        {
            Debug.LogWarning("JSON loading failed, generating sample data...");
            GenerateSampleData();
        }

        originalPlayers = new List<PlayerData>();
        foreach (var player in tournamentData.players)
        {
            originalPlayers.Add(player.Clone());
        }

        SortAndRankPlayers();
    }

    private void GenerateSampleData()
    {
        tournamentData = new TournamentData();

        int meScore = Random.Range(400, 1200);
        tournamentData.players.Add(new PlayerData("me", "Me", meScore));

        string[] sampleNames =
        {
            "DragonSlayer",
            "ShadowHunter",
            "FireWizard",
            "IceQueen",
            "ThunderLord",
            "MysticWarrior",
            "CrystalMage",
            "StormRider",
            "PhoenixKnight",
            "VoidMaster",
            "LightningBolt",
            "FrostGiant",
            "BlazeFury",
            "WindWalker",
            "EarthShaker",
            "StarCrusher",
            "MoonBlade",
            "SunBurst",
            "NightCrawler",
            "DayBreaker",
            "SuperLongPlayerNameThatShouldBeTruncated",
            "AnotherVeryLongName",
            "XxDarkDestroyerxX",
            "Player",
            "Gamer",
            "Champion",
            "Legend",
            "Master"
        };

        for (int i = 1; i < 1000; i++)
        {
            string id = $"user_{i:000}";
            string nickname =
                sampleNames[Random.Range(0, sampleNames.Length)] + Random.Range(1, 999);
            int score = Random.Range(200, 2500);

            tournamentData.players.Add(new PlayerData(id, nickname, score));
        }
    }

    public List<PlayerData> GetSortedPlayers()
    {
        return tournamentData.players.OrderByDescending(p => p.score).ToList();
    }

    public PlayerData GetCurrentPlayer()
    {
        return tournamentData.players.FirstOrDefault(p => p.IsCurrentPlayer);
    }

    public void UpdateScoresRandomly()
    {
        var mePlayer = GetCurrentPlayer();

        foreach (var player in tournamentData.players)
        {
            if (player.IsCurrentPlayer)
            {
                if (Random.Range(0f, 1f) < 0.8f)
                {
                    int increase = Random.Range(50, 400);
                    player.score += increase;
                }
            }
            else
            {
                if (Random.Range(0f, 1f) < 0.25f)
                {
                    int increase = Random.Range(5, 150);
                    player.score += increase;
                }
            }
        }

        SortAndRankPlayers();
    }

    private void SortAndRankPlayers()
    {
        var sortedPlayers = tournamentData.players.OrderByDescending(p => p.score).ToList();

        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            sortedPlayers[i].rank = i + 1;
        }

        tournamentData.players = sortedPlayers;
    }

    public int GetPlayerRank(string playerId)
    {
        var player = tournamentData.players.FirstOrDefault(p => p.id == playerId);
        return player?.rank ?? -1;
    }
}
