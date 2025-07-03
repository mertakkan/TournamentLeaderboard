using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
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
        if (jsonFile != null)
        {
            string jsonString = jsonFile.text;
            tournamentData = JsonUtility.FromJson<TournamentData>(jsonString);
        }
        else
        {
            // Generate sample data if no JSON file is provided
            GenerateSampleData();
        }

        // Store original data for reference
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

        // Add the "me" player with a random starting rank for better demo
        int meScore = Random.Range(400, 1200); // Lower initial score for better rank changes
        tournamentData.players.Add(new PlayerData("me", "Me", meScore));

        // More realistic player names for testing
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

        // Generate 999 other players with wider score range
        for (int i = 1; i < 1000; i++)
        {
            string id = $"user_{i:000}";
            string nickname =
                sampleNames[Random.Range(0, sampleNames.Length)] + Random.Range(1, 999);
            int score = Random.Range(200, 2500); // Wider range for better rank distribution

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
                // Give "me" player a bigger chance for significant score increases
                if (Random.Range(0f, 1f) < 0.8f) // 80% chance for me to get an update
                {
                    int increase = Random.Range(50, 400); // Larger increases for dramatic rank changes
                    player.score += increase;
                }
            }
            else
            {
                // Other players get smaller, less frequent updates
                if (Random.Range(0f, 1f) < 0.25f) // 25% chance for others
                {
                    int increase = Random.Range(5, 150); // Smaller increases
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
