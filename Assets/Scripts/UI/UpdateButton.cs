using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UpdateButton : MonoBehaviour
{
    [Header("Visual Components")]
    [SerializeField]
    private SpriteRenderer backgroundRenderer;

    [SerializeField]
    private SpriteRenderer iconRenderer;

    [SerializeField]
    private TextMeshPro buttonText;

    [Header("Animation Settings")]
    [SerializeField]
    private float hoverScale = 1.05f;

    [SerializeField]
    private float clickScale = 0.95f;

    [SerializeField]
    private float animationDuration = 0.15f;

    [Header("Colors")]
    [SerializeField]
    private Color normalColor = new Color(0.3f, 0.7f, 1f, 1f);

    [SerializeField]
    private Color hoverColor = new Color(0.4f, 0.8f, 1f, 1f);

    [SerializeField]
    private Color pressedColor = new Color(0.2f, 0.6f, 0.9f, 1f);

    [SerializeField]
    private Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    public event Action OnButtonClicked;

    private Vector3 originalScale;
    private bool isInteractable = true;
    private BoxCollider2D buttonCollider;

    void Awake()
    {
        originalScale = transform.localScale;
        buttonCollider = GetComponent<BoxCollider2D>();
        SetupButton();
    }

    private void SetupButton()
    {
        if (backgroundRenderer != null)
        {
            backgroundRenderer.color = normalColor;
            backgroundRenderer.sortingOrder = 5;

            backgroundRenderer.transform.localScale = new Vector3(4f, 1.5f, 1f);
        }

        if (iconRenderer != null)
        {
            iconRenderer.color = Color.white;
            iconRenderer.sortingOrder = 10;
            iconRenderer.transform.localPosition = new Vector3(-0.5f, 0, -0.1f);
        }

        if (buttonText != null)
        {
            buttonText.text = "UPDATE";
            buttonText.fontSize = 8f;
            buttonText.fontStyle = FontStyles.Bold;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = Color.white;
            buttonText.sortingOrder = 10;
            buttonText.transform.localPosition = new Vector3(0f, 0, -0.1f);
        }

        if (buttonCollider == null)
        {
            buttonCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        buttonCollider.size = new Vector3(4f, 1.5f);
    }

    void OnMouseDown()
    {
        if (!isInteractable)
            return;

        backgroundRenderer.color = pressedColor;
        transform.DOScale(originalScale * clickScale, animationDuration).SetEase(Ease.OutQuart);

        if (iconRenderer != null)
        {
            iconRenderer.transform.DORotate(new Vector3(0, 0, 180), animationDuration);
        }
    }

    void OnMouseUpAsButton()
    {
        if (!isInteractable)
            return;

        backgroundRenderer.color = normalColor;
        transform.DOScale(originalScale, animationDuration).SetEase(Ease.OutBack);

        if (iconRenderer != null)
        {
            iconRenderer.transform.DORotate(Vector3.zero, animationDuration);
        }

        OnButtonClicked?.Invoke();
    }

    void OnMouseEnter()
    {
        if (!isInteractable)
            return;

        backgroundRenderer.color = hoverColor;
        transform.DOScale(originalScale * hoverScale, animationDuration).SetEase(Ease.OutQuart);
    }

    void OnMouseExit()
    {
        if (!isInteractable)
            return;

        backgroundRenderer.color = normalColor;
        transform.DOScale(originalScale, animationDuration).SetEase(Ease.OutQuart);
    }

    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;

        Color targetColor = interactable ? normalColor : disabledColor;
        Color textColor = interactable ? Color.white : new Color(0.7f, 0.7f, 0.7f);

        backgroundRenderer.color = targetColor;
        buttonText.color = textColor;

        if (iconRenderer != null)
        {
            iconRenderer.color = textColor;
        }
    }

    public void PlayLoadingAnimation()
    {
        if (iconRenderer != null)
        {
            iconRenderer
                .transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        }
    }

    public void StopLoadingAnimation()
    {
        if (iconRenderer != null)
        {
            iconRenderer.transform.DOKill();
            iconRenderer.transform.rotation = Quaternion.identity;
        }
    }
}
