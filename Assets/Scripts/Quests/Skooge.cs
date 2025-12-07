using DG.Tweening;
using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Skooge : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private QuestInstance currentQuest;
    [SerializeField] private float holdTime;
    [SerializeField] private float holdTimeRequired;
    [SerializeField] private bool itemIsStaying;
    [SerializeField] private float shakeStrength = 0.05f;
    [SerializeField] private float uiShakeStrength;
    private SpriteRenderer spriteRenderer;
    private Vector3 originalPosition;
    [SerializeField] private GameObject questCanvas;
    [SerializeField] private GameObject questItemPrefab;
    private GameObject[] questItemsIcons;
    [SerializeField] private BarkManager barkManager;

    [SerializeField] protected GameObject enteredItem;
    [SerializeField] protected bool isHoldingItem;
    [SerializeField] protected bool isHoveringItem;
    [SerializeField] private StartLever startLever;
    private int day;

    [SerializeField] private Sprite[] sortingIcons;

    public QuestInstance GetCurrentQuest()
    {
        return currentQuest;
    }

    public ItemSO[] GetQuestItems() => currentQuest.questData.questItems;
    public bool GetIsItemStaying() => itemIsStaying;
    
    public float[] GetHoldTimeInfo()
    {
        float[] stats = { holdTime, holdTimeRequired };
        return stats;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentQuest.IsComplete())
        {
            return;
        }

        if (collision.CompareTag("Item"))
        {
            var draggable = collision.GetComponent<Draggable>();

            if (!draggable.GetHeld())
            {
                return;
            }

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
        if (currentQuest.IsComplete())
        {
            return;
        }
        var draggable = collision.GetComponent<Draggable>();

        if (!draggable.GetHeld())
        {
            return;
        }

        holdTime += Time.deltaTime;
        itemIsStaying = true;

        if (holdTime >= 5f)
        {

            if (draggable != null)
            {
                HandleItemRelease(draggable);
            }
        }
    }


    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (currentQuest.IsComplete())
        {
            return;

        }
        if (collision.CompareTag("Item"))
        {
            var draggable = collision.GetComponent<Draggable>();
            if (draggable != null)
            {
                draggable.OnReleased -= HandleItemRelease;
            }

            spriteRenderer.enabled = false;
            if (!DOTween.IsTweening(gameObject))
            {
                transform.DOShakePosition(shakeStrength, 0.1f).OnComplete(() => transform.DOMove(originalPosition, 0.1f));
            }

            enteredItem = null;
            isHoldingItem = false;
            isHoveringItem = false;
            holdTime = 0;
            itemIsStaying = false;
        }
    }
    public void HandleItemRelease(Draggable draggable)
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

        if (isHoldingItem && draggable.gameObject != enteredItem)
        {
            draggable.ResetPosition();
            return;
        }

        if (!isHoveringItem)
        {
            return;
        }
        enteredItem = draggable.gameObject;
        isHoldingItem = true;

        Item item = draggable.GetComponent<Item>();
        if (item == null)
        {
            return;
        }
        if (currentQuest.isActive && currentQuest.questData.questItems.Contains(item.GetItemData()) && !currentQuest.progress[Array.IndexOf(currentQuest.questData.questItems, item.GetItemData())])
        {
            //will autocheck for completion in the QuestInstance
            int index = Array.IndexOf(currentQuest.questData.questItems, item.GetItemData());

            currentQuest.UpdateProgress(index, true);

            if (questItemsIcons[index] != null)
            {
                Destroy(questItemsIcons[index]);
                questItemsIcons[index] = null;
            }
            if(currentQuest.IsComplete())
            {
                questCanvas.SetActive(false);
                StartCoroutine(SkoogeBark());
            }

            enteredItem = null;
            StartCoroutine(ShrinkItem(draggable.gameObject));

        }
        else
        {
            item.ResetPosition();
        }
    }
    private IEnumerator SkoogeBark()
    {
        barkManager.StartSkoogeBark();
        yield return new WaitForSeconds(2f);
        spriteRenderer.enabled = false;
    }
    private IEnumerator ShrinkItem(GameObject item)
    {
        float shrinkDuration = 0.1f;
        float startScale = 1f;
        float targetScale = 0.2f;
        float timeElapsed = 0f;

        Vector3 originalPosition = item.transform.localPosition;

        while (timeElapsed < shrinkDuration)
        {
            timeElapsed += Time.deltaTime;
            float newScale = Mathf.Lerp(startScale, targetScale, timeElapsed / shrinkDuration);
            item.transform.localScale = new Vector3(newScale, newScale, newScale);

            item.transform.localPosition = originalPosition;

            yield return null;
        }

        item.transform.localScale = new Vector3(targetScale, targetScale, targetScale);

        Destroy(item);
    }


    private void Start()
    {
        day = StatsController.Instance.GetDays();
        if (day < 3)
        {
            gameObject.SetActive(false);
            questCanvas.SetActive(false);
        }
        startLever.OnGameStart += HandleGameStart;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        originalPosition = transform.position;
        questCanvas.SetActive(false);



    }
    private void HandleGameStart(StartLever lever)
    {
        StartDailyQuest(day);
    }

    public void StartDailyQuest(int day)
    {
        if (day < 3)
        {
            return;
        }
        SkoogeQuestSO[] quests = Resources.LoadAll<SkoogeQuestSO>("").Where(quests => quests.day == day).ToArray();
        SkoogeQuestSO questData = quests[UnityEngine.Random.Range(0, quests.Length)];
        questItemsIcons = new GameObject[questData.questItems.Length];
        questCanvas.SetActive(true);
        if (!DOTween.IsTweening(questCanvas.gameObject))
        {
            Vector3 originalPosition = questCanvas.gameObject.transform.position;
            questCanvas.GetComponent<RectTransform>().DOShakeAnchorPos(0.1f, uiShakeStrength).OnComplete(() => questCanvas.transform.position = originalPosition);
        }
        for (int i = 0; i < questData.questItems.Length; i++)
        {
            GameObject itemInstance = Instantiate(questItemPrefab, questCanvas.transform.position, Quaternion.identity);
            itemInstance.transform.SetParent(questCanvas.transform);
            Image image = itemInstance.GetComponent<Image>();
            Image iconImage = itemInstance.GetComponentInChildren<Image>();
            if (questData.questItems[i].Rarity == Rarity.Uncommon)
            {
                if (questData.questItems[i].Sorting == Sorting.Organic)
                {
                    iconImage.sprite = sortingIcons[0];
                }
                else if (questData.questItems[i].Sorting == Sorting.Fuel)
                {
                    iconImage.sprite = sortingIcons[1];
                }
                else if (questData.questItems[i].Sorting == Sorting.Scrap)
                {
                    iconImage.sprite = sortingIcons[2];
                }
            }
            image.sprite = questData.questItems[i].Sprite[0];
            questItemsIcons[i] = itemInstance;
        }
        
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

  

}
