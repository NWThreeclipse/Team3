using DG.Tweening;
using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Skooge : DragZone
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private QuestInstance currentQuest;
    [SerializeField] private float holdTime;
    [SerializeField] private float holdTimeRequired;
    [SerializeField] private bool itemIsStaying;
    [SerializeField] private float shakeStrength = 0.05f;
    private SpriteRenderer spriteRenderer;
    private Vector3 originalPosition;




    public ItemSO[] GetQuestItems() => currentQuest.questData.questItems;
    public bool GetIsItemStaying() => itemIsStaying;
    
    public float[] GetHoldTimeInfo()
    {
        float[] stats = { holdTime, holdTimeRequired };
        return stats;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Item"))
        {
            var draggable = collision.GetComponent<Draggable>();

            if (draggable != null)
            {
                draggable.OnReleased += HandleItemRelease;
            }
        }

        spriteRenderer.enabled = true;
        if (!DOTween.IsTweening(gameObject))
        {
            transform.DOShakePosition(shakeStrength, 0.1f).OnComplete(() => transform.DOMove(originalPosition, 0.1f));
        }

        isHoveringItem = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        holdTime += Time.deltaTime;
        itemIsStaying = true;

    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.CompareTag("Item"))
        {
            var draggable = collision.GetComponent<Draggable>();
            if (draggable != null)
            {
                draggable.OnReleased -= HandleItemRelease;
            }

            spriteRenderer.enabled = false;

            enteredItem = null;
            isHoldingItem = false;
            isHoveringItem = false;
            holdTime = 0;
            itemIsStaying = false;
        }
    }
    protected override void HandleItemRelease(Draggable draggable)
    {
        itemIsStaying = false;
        if (holdTime < 5f)
        {
            Item i = draggable.GetComponent<Item>();
            i.ResetPosition();
            return;
        }
        Collider2D[] hits = Physics2D.OverlapPointAll(draggable.transform.position);
        bool stillInBin = false;

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
            {
                stillInBin = true;
                break;
            }
        }
        if (!stillInBin)
        {
            return;
        }

        base.HandleItemRelease(draggable);

        Item item = draggable.GetComponent<Item>();
        if (item == null)
        {
            return;
        }
        if (currentQuest.isActive && currentQuest.questData.questItems.Contains(item.GetItemData()) && !currentQuest.progress[Array.IndexOf(currentQuest.questData.questItems, item.GetItemData())])
        {
            //will autocheck for completion in the QuestInstance
            currentQuest.UpdateProgress(Array.IndexOf(currentQuest.questData.questItems, item.GetItemData()), true);
            enteredItem = null;
            StartCoroutine(ShrinkItem(draggable.gameObject));
        }
        else
        {
            item.ResetPosition();
        }
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

    private void Start()
    {
        int day = StatsController.Instance.GetDays();
        if (day < 3)
        {
            gameObject.SetActive(false);
        }
        StartDailyQuest(day);
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        originalPosition = transform.position;


    }

    public void StartDailyQuest(int day)
    {
        if (day < 3)
        {
            return;
        }
        SkoogeQuestSO[] quests = Resources.LoadAll<SkoogeQuestSO>("").Where(quests => quests.day == day).ToArray();
        SkoogeQuestSO questData = quests[UnityEngine.Random.Range(0, quests.Length)];

        if (questData != null)
        {
            currentQuest = new QuestInstance(questData);
            Debug.Log("Started quest for Day " + day);
        }
        else
        {
            Debug.Log("No quest found for Day " + day);
        }

    }

    public void UpdateQuestProgress(int itemIndex, bool isCompleted)
    {
        if (currentQuest != null)
        {
            currentQuest.UpdateProgress(itemIndex, isCompleted);
        }
    }
}
