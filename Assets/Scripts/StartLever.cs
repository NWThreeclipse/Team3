using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class StartLever : MonoBehaviour
{
    public event Action<StartLever> OnGameStart;

    [SerializeField] private float shakeStrength = 0.05f;
    private bool gameStarted = false;
    private bool interactible = false;
    [SerializeField] private Sprite flippedSprite;
    [SerializeField] private AudioSource flipSource;
    [SerializeField] private SpriteRenderer glowRenderer;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float glowDuration = 5f;
    [SerializeField] private float maxScaleIncrease = 0.3f;
    [SerializeField] private DialogueManager dialogueManager;
    
    public void SetInteractible(bool toggle)
    {
        interactible = toggle;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        dialogueManager.OnDialogueEnd += StartGlow;
    }

    private void StartGlow(DialogueManager dialogueManager)
    {
        glowRenderer.enabled = true;
        StartCoroutine(GlowEffect());

    }
    private IEnumerator GlowEffect()
    {
        Vector3 originalScale = transform.localScale;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;

            float scaleLerp = Mathf.PingPong(timer / glowDuration, 1f);
            transform.localScale = originalScale + new Vector3(scaleLerp * maxScaleIncrease, scaleLerp * maxScaleIncrease, 0);

            Color color = glowRenderer.color;
            color.a = Mathf.PingPong(timer / glowDuration, 1f);
            glowRenderer.color = color;

            yield return null;
        }
    }
    private void OnMouseDown()
    {
        if (gameStarted || !interactible)
        {
            return;
        }
        Vector3 newpos = new Vector3(2.704f, 3.554f, 0f);
        transform.position = newpos;
        glowRenderer.enabled = false;
        StopAllCoroutines();
        spriteRenderer.sprite = flippedSprite;
        flipSource.Play();

        if (!DOTween.IsTweening(gameObject))
        {
            transform.DOShakePosition(shakeStrength, 0.1f).OnComplete(() => transform.position = newpos);
        }
        OnGameStart?.Invoke(this);
        gameStarted = true;
        interactible = false;
    }
}
