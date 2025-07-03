using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private LeaderboardView leaderboardViewPrefab;

    private LeaderboardView leaderboardView;

    void Start()
    {
        SetupGame();
    }

    private void SetupGame()
    {
        // Create leaderboard view
        if (leaderboardViewPrefab != null)
        {
            leaderboardView = Instantiate(leaderboardViewPrefab);
        }
        else
        {
            Debug.LogError("LeaderboardView prefab is not assigned!");
        }
    }
}
