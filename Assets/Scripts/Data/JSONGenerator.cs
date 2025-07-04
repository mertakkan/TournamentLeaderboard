using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JSONGenerator : MonoBehaviour
{
    [Header("JSON Generation Settings")]
    [SerializeField]
    private int totalPlayers = 1000;

    [SerializeField]
    private string fileName = "tournament_data.json";

    [ContextMenu("Generate JSON File")]
    public void GenerateJSONFile()
    {
        TournamentData tournamentData = new TournamentData();

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
            "Master",
            "Warrior",
            "Knight",
            "Mage",
            "Archer",
            "Rogue",
            "Paladin",
            "Sorcerer",
            "Ninja",
            "Samurai",
            "Viking",
            "Gladiator",
            "Barbarian",
            "Assassin",
            "Ranger",
            "Wizard",
            "Necromancer",
            "Demon",
            "Angel",
            "Phoenix",
            "Dragon"
        };

        for (int i = 1; i < totalPlayers; i++)
        {
            string id = $"user_{i:000}";
            string nickname =
                sampleNames[Random.Range(0, sampleNames.Length)] + Random.Range(1, 999);
            int score = Random.Range(200, 2500);

            tournamentData.players.Add(new PlayerData(id, nickname, score));
        }

        string json = JsonUtility.ToJson(tournamentData, true);

        string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }

        File.WriteAllText(streamingAssetsPath, json);

        Debug.Log($"JSON file generated successfully at: {streamingAssetsPath}");
        Debug.Log($"Generated {tournamentData.players.Count} players");

        string projectPath = Path.Combine(Application.dataPath, "StreamingAssets", fileName);
        File.WriteAllText(projectPath, json);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
