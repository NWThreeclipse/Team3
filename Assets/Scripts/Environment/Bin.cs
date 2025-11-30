using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Bin : MonoBehaviour
{
    [SerializeField] private Sorting sorting;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Bin[] otherBins;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite correctSprite;
    [SerializeField] private Sprite incorrectSprite;
    [SerializeField] private float shakeStrength = 0.05f;

    [SerializeField] private BarkManager barkManager;
    [SerializeField] private SupervisorController supervisor;


    private Vector3 originalPosition;
    private Vignette vin;

    private void Start()
    {
        originalPosition = transform.position;
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Item"))
        {
            return;
        }

        var draggable = collision.GetComponent<Draggable>();
        if (draggable == null)
        {
            return;
        }

        draggable.OnReleased += HandleItemRelease;

        spriteRenderer.sprite = hoverSprite;

        if (!DOTween.IsTweening(gameObject))
        {
            transform.DOShakePosition(shakeStrength, 0.1f).OnComplete(() => transform.DOMove(originalPosition, 0.1f));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Item"))
        {
            return;
        }

        var draggable = collision.GetComponent<Draggable>();
        if (draggable != null)
        {
            draggable.OnReleased -= HandleItemRelease;
        }

        spriteRenderer.sprite = defaultSprite;

        if (!DOTween.IsTweening(gameObject))
        {
            transform.DOShakePosition(shakeStrength, 0.1f);
        }
    }

    private void HandleItemRelease(Draggable draggable)
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, GetComponent<Collider2D>().bounds.size, 0f, LayerMask.GetMask("Item"));

        if (hit == null || hit.gameObject != draggable.gameObject)
        {
            draggable.ResetPosition();
            return;
        }

        Item item = draggable.GetComponent<Item>();
        if (item == null)
        {
            return;
        }
        if (!item.GetSortable())
        {
            draggable.ResetPosition();
            return;
        }

        foreach (var other in otherBins)
        {
            Collider2D overlap = Physics2D.OverlapBox(other.transform.position, other.GetComponent<Collider2D>().bounds.size, 0f, LayerMask.GetMask("Item"));

            if (overlap != null && overlap.gameObject == draggable.gameObject)
            {
                draggable.ResetPosition();
                return;
            }
        }
        if(item.GetItemData().Rarity == Rarity.Anomalous)
        {
            vin = item.GetVignette();

            if (vin.intensity.value != 0)
            {
                float vinIntensity = vin.intensity.value;
                StopCoroutine(item.GetVignetteCoroutine());
                StartCoroutine(FadeOutVignette(vinIntensity));
            }
        }
        
        StartCoroutine(PlayEffect(item));
        StartCoroutine(ShrinkItem(draggable.gameObject));
    }
    private IEnumerator ShrinkItem(GameObject item)
    {
        float shrinkDuration = 0.1f;
        float startScale = 1f;
        float targetScale = 0.2f;
        float timeElapsed = 0f;

        while (timeElapsed < shrinkDuration)
        {
            timeElapsed += Time.deltaTime;
            float newScale = Mathf.Lerp(startScale, targetScale, timeElapsed / shrinkDuration);
            item.transform.localScale = new Vector3(newScale, newScale, newScale);

            yield return null;
        }

        item.transform.localScale = new Vector3(targetScale, targetScale, targetScale);

        Destroy(item);
    }


    private IEnumerator FadeOutVignette(float intensity)
    {
        float targetIntensity = 0.6f;
        float fadeDuration = 1f;
        float startIntensity = intensity;
        float timeElapsed = 0f;
        
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            vin.intensity.value = Mathf.Lerp(targetIntensity, 0f, timeElapsed / fadeDuration);

            yield return null;
        }

    }

    private IEnumerator PlayEffect(Item item)
    {
        bool correct = item.GetItemData().Sorting == sorting;

        float delay = correct ? AudioController.PlayCorrectSound()
                              : AudioController.PlayIncorrectSound();

        yield return new WaitForSeconds(delay);

        if (correct)
        {
            gameManager.AddStat(sorting, item.GetItemData().Value);
            spriteRenderer.sprite = correctSprite;
            
        }
        else
        {
            gameManager.SubtractStat(sorting, item.GetItemData().Value);
            gameManager.CheckLoss();
            spriteRenderer.sprite = incorrectSprite;
        }
        bool[] stats = supervisor.GetInspecting();

        if (stats[1])
        {
            barkManager.StartSupervisorBark(stats[0], correct);
        }
        
        StatsController.Instance.IncrementItem(correct);
        if (item.GetItemData().Rarity == Rarity.Anomalous)
        {
            StatsController.Instance.IncrementAnomalousItem(item.GetItemData().ItemName);
        }

        yield return new WaitForSeconds(2f);
        spriteRenderer.sprite = defaultSprite;
    }
}
