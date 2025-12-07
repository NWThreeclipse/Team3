using DG.Tweening;
using System;
using UnityEngine;

public class StartLever : MonoBehaviour
{
    public event Action<StartLever> OnGameStart;

    [SerializeField] private float shakeStrength = 0.05f;
    private Vector3 originalPosition;
    private bool gameStarted = false;
    private bool interactible = false;
    [SerializeField] private Sprite flippedSprite;

    private SpriteRenderer spriteRenderer;

    
    public void SetInteractible(bool toggle)
    {
        interactible = toggle;
    }

    void Start()
    {
        originalPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        if (gameStarted || !interactible)
        {
            return;
        }
        Vector3 newpos = new Vector3(2.704f, 3.934f, 0f);
        transform.position = newpos;
        spriteRenderer.sprite = flippedSprite;

        if (!DOTween.IsTweening(gameObject))
        {
            transform.DOShakePosition(shakeStrength, 0.1f).OnComplete(() => transform.position = newpos);
        }
        OnGameStart?.Invoke(this);
        gameStarted = true;
        interactible = false;
    }
}
