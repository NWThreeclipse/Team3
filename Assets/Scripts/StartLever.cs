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

    
    public void SetInteractible(bool toggle)
    {
        interactible = toggle;
    }

    void Start()
    {
        originalPosition = transform.position;
    }

    private void OnMouseDown()
    {
        if (gameStarted || !interactible)
        {
            return;
        }
        if (!DOTween.IsTweening(gameObject))
        {
            transform.DOShakePosition(shakeStrength, 0.1f).OnComplete(() => transform.DOMove(originalPosition, 0.1f));
        }
        OnGameStart?.Invoke(this);
        gameStarted = true;
        interactible = false;
    }
}
