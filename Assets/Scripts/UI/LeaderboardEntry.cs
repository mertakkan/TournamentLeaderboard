using DG.Tweening;
using TMPro;
using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{
    [Header("Visual Components")]
    [SerializeField]
    private SpriteRenderer backgroundRenderer;

    [SerializeField]
    private SpriteRenderer rankBackgroundRenderer;

    [SerializeField]
    private SpriteRenderer profileIconRenderer;

    [SerializeField]
    private SpriteRenderer borderRenderer; // Add border for Clash of Clans style

    [SerializeField]
    private TextMeshPro rankText;

    [SerializeField]
    private TextMeshPro nicknameText;

    [SerializeField]
    private TextMeshPro scoreText;

    [Header("Colors - Clash of Clans Style")]
    [SerializeField]
    private Color normalColor = new Color(0.95f, 0.9f, 0.8f, 1f); // Warm beige

    [SerializeField]
    private Color currentPlayerColor = new Color(1f, 0.85f, 0.3f, 1f); // Golden

    [SerializeField]
    private Color topRankColor = new Color(0.9f, 0.95f, 1f, 1f); // Light blue

    [SerializeField]
    private Color borderColor = new Color(0.4f, 0.3f, 0.2f, 1f); // Dark brown border

    [Header("Rank Colors")]
    [SerializeField]
    private Color goldColor = new Color(1f, 0.84f, 0f);

    [SerializeField]
    private Color silverColor = new Color(0.75f, 0.75f, 0.75f);

    [SerializeField]
    private Color bronzeColor = new Color(0.8f, 0.5f, 0.2f);

    [SerializeField]
    private Color defaultRankColor = new Color(0.5f, 0.4f, 0.3f);

    [Header("Text Settings")]
    [SerializeField]
    private int maxNicknameLength = 10;

    [SerializeField]
    private float nicknameMaxWidth = 2.5f; // Maximum width for nickname text

    [Header("Rank Text Scaling")]
    [SerializeField]
    private float baseFontSize = 12f;

    [SerializeField]
    private float minFontSize = 6f;

    private PlayerData playerData;
    private bool isCurrentPlayer;
    private Vector3 originalScale;

    public PlayerData PlayerData => playerData;
    public bool IsCurrentPlayer => isCurrentPlayer;
    public string PlayerId => playerData?.id;

    void Awake()
    {
        originalScale = transform.localScale;
        SetupTextProperties();
        SetupVisualElements();
    }

    private void SetupTextProperties()
    {
        // Setup rank text with dynamic scaling
        if (rankText != null)
        {
            rankText.fontSize = baseFontSize;
            rankText.fontStyle = FontStyles.Bold;
            rankText.alignment = TextAlignmentOptions.Center;
            rankText.color = Color.white;
            rankText.sortingOrder = 12;
        }

        // Setup nickname text with proper constraints
        if (nicknameText != null)
        {
            nicknameText.fontSize = 6f;
            nicknameText.fontStyle = FontStyles.Bold;
            nicknameText.alignment = TextAlignmentOptions.Center;
            nicknameText.color = new Color(0.2f, 0.15f, 0.1f);
            nicknameText.sortingOrder = 12;
            nicknameText.enableWordWrapping = false;
            nicknameText.overflowMode = TextOverflowModes.Truncate;
        }

        // Setup score text
        if (scoreText != null)
        {
            scoreText.fontSize = 7f;
            scoreText.fontStyle = FontStyles.Bold;
            scoreText.alignment = TextAlignmentOptions.Center;
            scoreText.color = new Color(0.3f, 0.2f, 0.1f);
            scoreText.sortingOrder = 12;
        }
    }

    private void SetupVisualElements()
    {
        // Setup background
        if (backgroundRenderer != null)
        {
            backgroundRenderer.sortingOrder = 5;
        }

        // Setup border
        if (borderRenderer != null)
        {
            borderRenderer.color = borderColor;
            borderRenderer.sortingOrder = 6;
        }

        // Setup rank background
        if (rankBackgroundRenderer != null)
        {
            rankBackgroundRenderer.sortingOrder = 8;
        }

        // Setup profile icon
        if (profileIconRenderer != null)
        {
            profileIconRenderer.sortingOrder = 9;
        }
    }

    public void SetupEntry(PlayerData data, bool isCurrentPlayerEntry = false)
    {
        playerData = data;
        isCurrentPlayer = isCurrentPlayerEntry;

        UpdateVisuals();
        UpdateColors();
        PositionElements();
    }

    private void UpdateVisuals()
    {
        if (playerData == null)
            return;

        // Update rank with dynamic font scaling
        if (rankText != null)
        {
            rankText.text = playerData.rank.ToString();
            ScaleRankTextToFit();
        }

        // Update nickname with proper truncation
        if (nicknameText != null)
        {
            SetNicknameWithConstraints(playerData.nickname);
        }

        // Update score with proper formatting
        if (scoreText != null)
            scoreText.text = FormatScore(playerData.score);
    }

    private void ScaleRankTextToFit()
    {
        if (rankText == null || rankBackgroundRenderer == null)
            return;

        // Get the available width/height from the rank background
        float availableWidth = rankBackgroundRenderer.bounds.size.x * 0.8f; // 80% of background width
        float availableHeight = rankBackgroundRenderer.bounds.size.y * 0.8f; // 80% of background height

        // Start with base font size and scale down if needed
        float currentFontSize = baseFontSize;
        rankText.fontSize = currentFontSize;
        rankText.ForceMeshUpdate();

        // Check if text fits within the available space
        while (
            (rankText.preferredWidth > availableWidth || rankText.preferredHeight > availableHeight)
            && currentFontSize > minFontSize
        )
        {
            currentFontSize -= 0.5f;
            rankText.fontSize = currentFontSize;
            rankText.ForceMeshUpdate();
        }

        // Ensure minimum font size
        if (currentFontSize < minFontSize)
        {
            rankText.fontSize = minFontSize;
        }
    }

    private void SetNicknameWithConstraints(string nickname)
    {
        if (nicknameText == null)
            return;

        // First, try the original nickname
        nicknameText.text = nickname;
        nicknameText.ForceMeshUpdate();

        // If it's too wide, truncate it
        if (nicknameText.preferredWidth > nicknameMaxWidth)
        {
            string truncated = nickname;
            int length = nickname.Length;

            // Keep reducing until it fits
            while (nicknameText.preferredWidth > nicknameMaxWidth && length > 3)
            {
                length--;
                truncated = nickname.Substring(0, length) + "...";
                nicknameText.text = truncated;
                nicknameText.ForceMeshUpdate();
            }
        }
    }

    private string FormatScore(int score)
    {
        if (score >= 1000000)
            return $"{score / 1000000f:F1}M";
        else if (score >= 1000)
            return $"{score / 1000f:F1}K";
        else
            return score.ToString("N0");
    }

    private void PositionElements()
    {
        if (backgroundRenderer == null)
            return;

        float bgWidth = backgroundRenderer.bounds.size.x;
        float bgHeight = backgroundRenderer.bounds.size.y;

        // Position rank elements on the far left
        if (rankBackgroundRenderer != null)
        {
            rankBackgroundRenderer.transform.localPosition = new Vector3(-bgWidth * 0.4f, 0, 0);
        }
        if (rankText != null)
        {
            rankText.transform.localPosition = new Vector3(-bgWidth * 0.4f, 0, -0.1f);
        }

        // Position profile icon
        if (profileIconRenderer != null)
        {
            profileIconRenderer.transform.localPosition = new Vector3(-bgWidth * 0.2f, 0, 0);
        }

        // Position nickname with proper spacing
        if (nicknameText != null)
        {
            nicknameText.transform.localPosition = new Vector3(-bgWidth * 0.05f, 0, -0.1f);
        }

        // Position score on the right
        if (scoreText != null)
        {
            scoreText.transform.localPosition = new Vector3(bgWidth * 0.4f, 0, -0.1f);
        }
    }

    private void UpdateColors()
    {
        Color bgColor = normalColor;
        Color textColor = new Color(0.2f, 0.15f, 0.1f);

        if (isCurrentPlayer)
        {
            bgColor = currentPlayerColor;
            textColor = new Color(0.1f, 0.05f, 0.0f);
        }
        else if (playerData != null && playerData.rank <= 3)
        {
            bgColor = topRankColor;
            textColor = new Color(0.1f, 0.1f, 0.2f);
        }

        // Apply colors
        if (backgroundRenderer != null)
            backgroundRenderer.color = bgColor;

        if (nicknameText != null)
            nicknameText.color = textColor;

        if (scoreText != null)
            scoreText.color = textColor;

        // Update rank colors
        if (rankBackgroundRenderer != null && playerData != null)
        {
            rankBackgroundRenderer.color = GetRankColor(playerData.rank);
        }

        if (profileIconRenderer != null && playerData != null)
        {
            profileIconRenderer.color = GetProfileIconColor(playerData.rank);
        }

        // Update border for current player
        if (borderRenderer != null)
        {
            if (isCurrentPlayer)
            {
                borderRenderer.color = new Color(1f, 0.8f, 0.2f, 1f); // Golden border
            }
            else
            {
                borderRenderer.color = borderColor;
            }
        }
    }

    private Color GetRankColor(int rank)
    {
        switch (rank)
        {
            case 1:
                return goldColor;
            case 2:
                return silverColor;
            case 3:
                return bronzeColor;
            default:
                return defaultRankColor;
        }
    }

    private Color GetProfileIconColor(int rank)
    {
        if (rank <= 10)
            return new Color(0.9f, 0.7f, 0.3f);
        else if (rank <= 100)
            return new Color(0.7f, 0.7f, 0.9f);
        else
            return new Color(0.6f, 0.6f, 0.6f);
    }

    public Tween AnimateToPosition(Vector3 targetPosition, float duration = 0.5f)
    {
        return transform.DOMove(targetPosition, duration).SetEase(Ease.OutQuart);
    }

    public Tween AnimateScale(float scale, float duration = 0.3f)
    {
        return transform.DOScale(originalScale * scale, duration).SetEase(Ease.OutBack);
    }

    public void ResetScale()
    {
        transform.localScale = originalScale;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void UpdateData(PlayerData newData)
    {
        playerData = newData;
        UpdateVisuals();
        UpdateColors();
    }

    public Tween PlayEntryAnimation()
    {
        Vector3 startScale = Vector3.zero;
        transform.localScale = startScale;

        return transform
            .DOScale(originalScale, 0.3f)
            .SetEase(Ease.OutBack)
            .SetDelay(Random.Range(0f, 0.1f));
    }

    public Sequence PlayHighlightAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(originalScale * 1.1f, 0.2f));
        sequence.Append(transform.DOScale(originalScale, 0.2f));
        sequence.SetEase(Ease.OutQuart);
        return sequence;
    }
}
