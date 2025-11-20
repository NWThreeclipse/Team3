using DG.Tweening;
using System.Collections;
using UnityEngine;

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

    private Vector3 originalPosition;

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

        foreach (var other in otherBins)
        {
            Collider2D overlap = Physics2D.OverlapBox(other.transform.position, other.GetComponent<Collider2D>().bounds.size, 0f, LayerMask.GetMask("Item"));

            if (overlap != null && overlap.gameObject == draggable.gameObject)
            {
                draggable.ResetPosition();
                return;
            }
        }

        StartCoroutine(PlayEffect(item));
        Destroy(draggable.gameObject);
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

        StatsController.Instance.IncrementItem(correct);

        yield return new WaitForSeconds(2f);
        spriteRenderer.sprite = defaultSprite;
    }
}
